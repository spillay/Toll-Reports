using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Toll.Reporting.Api.DTOs;
using TollReportingSystem.Data;

public class ComprehensiveRepository : IComprehensiveRepository
{
    private readonly ApplicationDbContext _context;

    public ComprehensiveRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ComprehensiveDto>> GetComprehensiveRepositoryAsync2(DateTime startDate, DateTime endDate, string paymentMethods)
    {
        var result = new List<ComprehensiveDto>();

        // Fetch transactions
        var transactions = await _context.Transactions
            .Where(t => t.TransactionDateTime >= startDate && t.TransactionDateTime <= endDate)
            .ToListAsync(); // materialize the query first

        //if (!string.IsNullOrEmpty(paymentMethods))
        //{
        //    transactions = transactions
        //        .Where(t => t.PaymentMethod == paymentMethods)
        //        .ToList();
        //}

        // Load lookup tables into dictionaries for O(1) access
        var laneDict = (await _context.Lanes.ToListAsync()).ToDictionary(l => l.LaneId, l => l.LaneName);
        var transTypeDict = (await _context.TransactionTypes.ToListAsync()).ToDictionary(t => t.TransactionTypeId, t => t.Description);
        var discountDict = (await _context.DiscountTypes.ToListAsync()).ToDictionary(d => d.DiscountTypeId, d => d.Description);
        var shiftDict = (await _context.Shifts.ToListAsync()).ToDictionary(s => s.ShiftId, s => s.Description);
        var userDict = (await _context.SystemUsers.ToListAsync()).ToDictionary(u => u.SystemUserId, u => u.Username);
        var tariffPlanDict = (await _context.TariffPlans.ToListAsync()).ToDictionary(tp => tp.TariffPlanId);

        var tariffPlanDetailDict = (await _context.TariffPlanDetails
        .GroupBy(tpd => tpd.TariffPlanId)
        .Select(g => g.First())
        .ToListAsync())
        .ToDictionary(tpd => tpd.TariffPlanId);


        var tollClassDict = (await _context.TollClasses.ToListAsync()).ToDictionary(tc => tc.TollClassId, tc => tc.ClassDescription);
        int count = 0;
        // Build result
        foreach (var t in transactions)
        {
            if (laneDict.TryGetValue(t.LaneId, out var laneName) &&
                transTypeDict.TryGetValue(t.TransactionTypeId, out var transType) &&
                discountDict.TryGetValue(t.DiscountTypeId, out var discountType) &&
                shiftDict.TryGetValue(t.ShiftId, out var shift) &&
                userDict.TryGetValue((long)t.SystemUserId, out var username) &&
                tariffPlanDict.TryGetValue(t.TariffPlanId, out var tp) &&
                tollClassDict.TryGetValue(t.ManualTollClassId, out var manualClass))
            {
                // find tariff plan detail (if any)
                var tpd = tariffPlanDetailDict.Values.FirstOrDefault(x => x.TariffPlanId == tp.TariffPlanId);

                result.Add(new ComprehensiveDto
                {
                    LaneName = laneName,
                    TransactionNumber = t.TransactionNumber,
                    TransactionType = transType,
                    DiscountType = discountType,
                    TransactionDateTime = t.TransactionDateTime,
                    Shift = shift,
                    ShiftDate = t.ShiftDate,
                    Username = username,
                    ManualTollClass = manualClass,
                    TariffPlanId = tp.TariffPlanId,
                    EffectiveDate = tp.EffectiveDate,
                    CurrencyId = tp.CurrencyId,
                    AmountInclusive = tpd?.AmountInclusive ?? 0
                });
            }
            if(count < 100)
            {
                count++;
            }
            else { break; }
        }

        return result;
    }

    public async Task<IEnumerable<ComprehensiveDto>> GetComprehensiveRepositoryAsync(DateTime startDate, DateTime endDate, string paymentMethods)
    {
        var result = new List<ComprehensiveDto>();

        // Fetch transactions
        var transactions = await _context.Transactions
            .Where(t => t.TransactionDateTime >= startDate && t.TransactionDateTime <= endDate)
            .ToListAsync();

        // Load lookup tables into dictionaries for O(1) access
        var laneDict = (await _context.Lanes.ToListAsync()).ToDictionary(l => l.LaneId, l => l.LaneName);
        var transTypeDict = (await _context.TransactionTypes.ToListAsync()).ToDictionary(t => t.TransactionTypeId, t => t.Description);
        var discountDict = (await _context.DiscountTypes.ToListAsync()).ToDictionary(d => d.DiscountTypeId, d => d.Description);
        var shiftDict = (await _context.Shifts.ToListAsync()).ToDictionary(s => s.ShiftId, s => s.Description);
        var userDict = (await _context.SystemUsers.ToListAsync()).ToDictionary(u => u.SystemUserId, u => u.Username);
        var tariffPlanDict = (await _context.TariffPlans.ToListAsync()).ToDictionary(tp => tp.TariffPlanId);

        // Use a lookup for multiple details per plan
        var tariffPlanDetailLookup = (await _context.TariffPlanDetails.ToListAsync())
            .ToLookup(tpd => tpd.TariffPlanId);

        var tollClassDict = (await _context.TollClasses.ToListAsync()).ToDictionary(tc => tc.TollClassId, tc => tc.ClassDescription);

        int count = 0;

        foreach (var t in transactions)
        {
            if (laneDict.TryGetValue(t.LaneId, out var laneName) &&
                transTypeDict.TryGetValue(t.TransactionTypeId, out var transType) &&
                discountDict.TryGetValue(t.DiscountTypeId, out var discountType) &&
                shiftDict.TryGetValue(t.ShiftId, out var shift) &&
                userDict.TryGetValue((long)t.SystemUserId, out var username) &&
                tariffPlanDict.TryGetValue(t.TariffPlanId, out var tp) &&
                tollClassDict.TryGetValue(t.ManualTollClassId, out var manualClass))
            {
                // Get all details for this tariff plan using the lookup
                var details = tariffPlanDetailLookup[t.TariffPlanId];

                // Create one DTO per AmountInclusive
                foreach (var d in details)
                {
                    result.Add(new ComprehensiveDto
                    {
                        LaneName = laneName,
                        TransactionNumber = t.TransactionNumber,
                        TransactionType = transType,
                        DiscountType = discountType,
                        TransactionDateTime = t.TransactionDateTime,
                        Shift = shift,
                        ShiftDate = t.ShiftDate,
                        Username = username,
                        ManualTollClass = manualClass,
                        TariffPlanId = tp.TariffPlanId,
                        EffectiveDate = tp.EffectiveDate,
                        CurrencyId = tp.CurrencyId,
                        AmountInclusive = d.AmountInclusive // single value per DTO
                    });
                }
            }

            if (count++ >= 100) break; // optional: limit for testing
        }

        return result;
    }


    public async Task<IEnumerable<ComprehensiveDto>> GetComprehensiveRepositoryAsync1(DateTime startDate, DateTime endDate, string paymentMethods)
    {
        var result = new List<ComprehensiveDto>();
        // 1. Get raw data from DB
        var transactions = await _context.Transactions
            .Where(t => t.TransactionDateTime >= startDate && t.TransactionDateTime <= endDate)
            .ToListAsync(); // materialize the query first

        var lanes = await _context.Lanes.ToListAsync();
        var transTypes = await _context.TransactionTypes.ToListAsync();
        var discountTypeIds = await _context.DiscountTypes.ToListAsync();
        var shifts = await _context.Shifts.ToListAsync();
        var systemUsers = await _context.SystemUsers.ToListAsync();
        var tariffPlans = await _context.TariffPlans.ToListAsync();
        var tariffPlanDetails = await _context.TariffPlanDetails.ToListAsync();
        var tollClasses = await _context.TollClasses.ToListAsync();


        var trans = transactions.Select(t => new ComprehensiveDto
        {
            LaneId = t.LaneId,
            TransactionNumber = t.TransactionNumber,
            TransactionDateTime = t.TransactionDateTime,
            ShiftDate = t.ShiftDate,
            ShiftId = t.ShiftId,
            SystemUserId = t.SystemUserId,
            TariffPlanId = t.TariffPlanId,
            ManualTollClassId = t.ManualTollClassId,
            TransactionTypeId = t.TransactionTypeId,
            DiscountTypeId = t.DiscountTypeId,
        });
        var transType = transTypes.Select(t => new ComprehensiveDto
        {
            TransactionTypeId = t.TransactionTypeId,
            TransactionType = t.Description

        });
        var lane = lanes.Select(l => new ComprehensiveDto
        {
            LaneId = l.LaneId,
            LaneName = l.LaneName
        });
        var discountType = discountTypeIds.Select(dt => new ComprehensiveDto
        {
            DiscountTypeId = dt.DiscountTypeId,
            DiscountType = dt.Description

        });

        var shift = shifts.Select(s => new ComprehensiveDto
        {
            ShiftId = s.ShiftId,
            Shift = s.Description
        });
        var systemUser = systemUsers.Select(su => new ComprehensiveDto
        {
            SystemUserId = su.SystemUserId,
            Username =su.Username

        });
        var tariffPlan = tariffPlans.Select(tp => new ComprehensiveDto
        {
            TariffPlanId = tp.TariffPlanId,
            EffectiveDate = tp.EffectiveDate,
            CurrencyId = tp.CurrencyId

        });
        var tariffPlanDetail = tariffPlanDetails.Select(tpd => new ComprehensiveDto
        {
            TariffPlanDetailId = tpd.TariffPlanId,
            AmountInclusive = tpd.AmountInclusive

        });
        var tollClass = tollClasses.Select(tc => new ComprehensiveDto
        {
            ManualTollClassId = tc.TollClassId,
            ManualTollClass = tc.ClassDescription

        });

        foreach (var t in trans)
        {
            var l = lane.FirstOrDefault(x => x.LaneId == t.LaneId);
            var tt = transType.FirstOrDefault(x => x.TransactionTypeId == t.TransactionTypeId);
            var dt = discountType.FirstOrDefault(x => x.DiscountTypeId == t.DiscountTypeId);
            var s = shift.FirstOrDefault(x => x.ShiftId == t.ShiftId);
            var su = systemUser.FirstOrDefault(x => x.SystemUserId == t.SystemUserId);
            foreach(var tp in tariffPlan)
            {
                //var tp = tariffPlan.FirstOrDefault(x => x.tariffPlanId == t.TariffPlanId);
                var tpd = tariffPlanDetail.FirstOrDefault(x => x.TariffPlanId == tp.TariffPlanId);
                var tc = tollClass.FirstOrDefault(x => x.ManualTollClassId == t.ManualTollClassId);

                // Only add if everything is found
                if (l != null && tt != null && dt != null && s != null && su != null && tp != null && tpd != null && tc != null)
                {
                    result.Add(new ComprehensiveDto
                    {
                        LaneName = l.LaneName,
                        TransactionNumber = t.TransactionNumber,
                        TransactionType = tt.TransactionType,
                        DiscountType = dt.DiscountType,
                        TransactionDateTime = t.TransactionDateTime,
                        Shift = s.Shift,
                        ShiftDate = t.ShiftDate,
                        Username = su.Username,
                        ManualTollClass = tc.ManualTollClass,
                        TariffPlanId = tp.TariffPlanId,
                        EffectiveDate = tp.EffectiveDate,
                        CurrencyId = tp.CurrencyId,
                        AmountInclusive = tpd.AmountInclusive
                    });
                }
            }
        }

        //foreach (var t in trans)
        //{
        //    // Check if the transaction's LaneId exists in the lanes collection
        //    if (lane.Any(l => l.LaneId == t.LaneId))
        //    {
        //        Console.WriteLine($"Transaction LaneId {t.LaneId} exists in lanes");
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Transaction LaneId {t.LaneId} NOT found in lanes");
        //    }
        //}


        //// 2. Map in memory
        //var paymentClassData = transactions.Select(t => new ComprehensiveDto
        //{
        //    MethodOfPayment = t.TransactionTypeId switch
        //    {
        //        1 => "Cash Transaction",
        //        2 => "Prepaid Transaction - RFID Tag",
        //        3 => "Prepaid Transaction - Card",
        //        4 => "Exempt Transaction",
        //        5 => "Violation",
        //        6 => "Bank Card(NFC - Post Paid)",
        //        7 => "QR Code / Barcode",
        //        8 => "Convoy"
        //    },
   

        //    Classification = t.ActualTollClassId.HasValue ?
        //         (t.ActualTollClassId.Value == 0 ? "Class 0" :
        //          t.ActualTollClassId.Value == 1 ? "Class 1" :
        //          t.ActualTollClassId.Value == 2 ? "Class 2" :
        //          t.ActualTollClassId.Value == 3 ? "Class 3" :
        //          t.ActualTollClassId.Value == 4 ? "Class M" : "Unknown")
        //         : "Unknown",

        //    Revenue = (decimal?)t.NettAmount ?? 0
        //}).ToList();






        //// Apply optional filter (paymentMethods)
        //if (paymentMethods != null && paymentMethods.Any())
        //{
        //    paymentClassData = paymentClassData
        //        .Where(x => paymentMethods.Contains(x.MethodOfPayment))
        //        .ToList();
        //}

        //// Aggregate by MethodOfPayment
        ////var aggregated = paymentClassData
        ////    .GroupBy(p => p.MethodOfPayment)
        ////    .Select(g => new
        ////    {
        ////        MethodOfPayment = g.Key,
        ////        ClassI_Count = g.Count(x => x.FinalClass == "Class I"),
        ////        ClassII_Count = g.Count(x => x.FinalClass == "Class II"),
        ////        ClassIII_Count = g.Count(x => x.FinalClass == "Class III"),
        ////        ClassM_Count = g.Count(x => x.FinalClass == "Class M"),
        ////        ClassI_Revenue = g.Where(x => x.FinalClass == "Class I").Sum(x => x.Revenue),
        ////        ClassII_Revenue = g.Where(x => x.FinalClass == "Class II").Sum(x => x.Revenue),
        ////        ClassIII_Revenue = g.Where(x => x.FinalClass == "Class III").Sum(x => x.Revenue),
        ////        ClassM_Revenue = g.Where(x => x.FinalClass == "Class M").Sum(x => x.Revenue)
        ////    })
        ////    .ToList();

        
        return result;

        // Format into Count, Count %, Revenue, Revenue %
        //foreach (var agg in aggregated)
        //{
        //    var totalCount = agg.ClassI_Count + agg.ClassII_Count + agg.ClassIII_Count + agg.ClassM_Count;
        //    var totalRevenue = agg.ClassI_Revenue + agg.ClassII_Revenue + agg.ClassIII_Revenue + agg.ClassM_Revenue;

        //    // Count
        //    result.Add(new ComprehensiveDto
        //    {
        //        MethodOfPayment = agg.MethodOfPayment,
        //        RowType = "Count",
        //        ClassI = agg.ClassI_Count,
        //        ClassII = agg.ClassII_Count,
        //        ClassIII = agg.ClassIII_Count,
        //        ClassM = agg.ClassM_Count
        //    });

        //    // Count %
        //    result.Add(new ComprehensiveDto
        //    {
        //        MethodOfPayment = agg.MethodOfPayment,
        //        RowType = "Count %",
        //        ClassI = totalCount == 0 ? 0 : Math.Round((decimal)agg.ClassI_Count / totalCount * 100, 2),
        //        ClassII = totalCount == 0 ? 0 : Math.Round((decimal)agg.ClassII_Count / totalCount * 100, 2),
        //        ClassIII = totalCount == 0 ? 0 : Math.Round((decimal)agg.ClassIII_Count / totalCount * 100, 2),
        //        ClassM = totalCount == 0 ? 0 : Math.Round((decimal)agg.ClassM_Count / totalCount * 100, 2),
        //        Total = 100
        //    });

        //    // Revenue
        //    result.Add(new ComprehensiveDto
        //    {
        //        MethodOfPayment = agg.MethodOfPayment,
        //        RowType = "Revenue",
        //        ClassI = agg.ClassI_Revenue,
        //        ClassII = agg.ClassII_Revenue,
        //        ClassIII = agg.ClassIII_Revenue,
        //        ClassM = agg.ClassM_Revenue,
        //        Total = totalRevenue
        //    });

        //    // Revenue %
        //    result.Add(new ComprehensiveDto
        //    {
        //        MethodOfPayment = agg.MethodOfPayment,
        //        RowType = "Revenue %",
        //        ClassI = totalRevenue == 0 ? 0 : Math.Round((decimal)agg.ClassI_Revenue / totalRevenue * 100, 2),
        //        ClassII = totalRevenue == 0 ? 0 : Math.Round((decimal)agg.ClassII_Revenue / totalRevenue * 100, 2),
        //        ClassIII = totalRevenue == 0 ? 0 : Math.Round((decimal)agg.ClassIII_Revenue / totalRevenue * 100, 2),
        //        ClassM = totalRevenue == 0 ? 0 : Math.Round((decimal)agg.ClassM_Revenue / totalRevenue * 100, 2),
        //        Total = 100
        //    });
        //}

        // Grand Total
        //var countRows = result.Where(r => r.RowType == "Count").ToList();
        //var revenueRows = result.Where(r => r.RowType == "Revenue").ToList();

        //var totalCountSum = countRows.Sum(r => r.Total);
        //var totalRevenueSum = revenueRows.Sum(r => r.Total);

        //result.Add(new ComprehensiveDto
        //{
        //    MethodOfPayment = "Grand Total",
        //    RowType = "Count",
        //    ClassI = countRows.Sum(r => r.ClassI),
        //    ClassII = countRows.Sum(r => r.ClassII),
        //    ClassIII = countRows.Sum(r => r.ClassIII),
        //    ClassM = countRows.Sum(r => r.ClassM),
        //    Total = totalCountSum
        //});

        //result.Add(new ComprehensiveDto
        //{
        //    MethodOfPayment = "Grand Total",
        //    RowType = "Count %",
        //    ClassI = totalCountSum == 0 ? 0 : Math.Round(countRows.Sum(r => r.ClassI) / totalCountSum * 100, 2),
        //    ClassII = totalCountSum == 0 ? 0 : Math.Round(countRows.Sum(r => r.ClassII) / totalCountSum * 100, 2),
        //    ClassIII = totalCountSum == 0 ? 0 : Math.Round(countRows.Sum(r => r.ClassIII) / totalCountSum * 100, 2),
        //    ClassM = totalCountSum == 0 ? 0 : Math.Round(countRows.Sum(r => r.ClassM) / totalCountSum * 100, 2),
        //    Total = 100
        //});

        //result.Add(new ComprehensiveDto
        //{
        //    MethodOfPayment = "Grand Total",
        //    RowType = "Revenue",
        //    ClassI = revenueRows.Sum(r => r.ClassI),
        //    ClassII = revenueRows.Sum(r => r.ClassII),
        //    ClassIII = revenueRows.Sum(r => r.ClassIII),
        //    ClassM = revenueRows.Sum(r => r.ClassM),
        //    Total = totalRevenueSum
        //});

        //result.Add(new ComprehensiveDto
        //{
        //    MethodOfPayment = "Grand Total",
        //    RowType = "Revenue %",
        //    ClassI = totalRevenueSum == 0 ? 0 : Math.Round(revenueRows.Sum(r => r.ClassI) / totalRevenueSum * 100, 2),
        //    ClassII = totalRevenueSum == 0 ? 0 : Math.Round(revenueRows.Sum(r => r.ClassII) / totalRevenueSum * 100, 2),
        //    ClassIII = totalRevenueSum == 0 ? 0 : Math.Round(revenueRows.Sum(r => r.ClassIII) / totalRevenueSum * 100, 2),
        //    ClassM = totalRevenueSum == 0 ? 0 : Math.Round(revenueRows.Sum(r => r.ClassM) / totalRevenueSum * 100, 2),
        //    Total = 100
        //});

        //// Return ordered (like SQL)
        //return result.OrderBy(r =>
        //    r.MethodOfPayment switch
        //    {
        //        "Cash Passage Transaction" => 1,
        //        "Foreign Cash Passage Transaction" => 2,
        //        "Prepaid Passage Transaction" => 3,
        //        "Postpaid Passage Transaction" => 4,
        //        "Exempt Passage Transaction" => 5,
        //        "Prepaid Top Up" => 6,
        //        "Violation Passage" => 7,
        //        "Grand Total" => 8,
        //        _ => 9
        //    })
            //.ThenBy(r => r.RowType switch
            //{
            //    "Count" => 1,
            //    "Count %" => 2,
            //    "Revenue" => 3,
            //    "Revenue %" => 4,
            //    _ => 5
            //})
            //.ToList();
    }

    public Task<IEnumerable<ComprehensiveDto>> GetComprehensiveRepositoryAsync1(DateTime startDate, DateTime endDate, List<string>? operationalShift = null, List<string>? tollOperators = null, List<string>? laneNames = null, List<string>? laneDiscountType = null, List<string>? Classification = null, List<string>? paymentMethods = null)
    {
        throw new NotImplementedException();
    }
}
