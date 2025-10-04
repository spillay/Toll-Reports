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


    public async Task<IEnumerable<ComprehensiveDto>> GetComprehensiveRepositoryAsync(DateTime startDate, DateTime endDate, string paymentMethods)
    {
        // 1. Get raw data from DB
        var transactions = await _context.Transactions
            .Where(t => t.TransactionDateTime >= startDate && t.TransactionDateTime <= endDate)
            .ToListAsync(); // materialize the query first

        // 2. Map in memory
        var paymentClassData = transactions.Select(t => new ComprehensiveDto
        {
            MethodOfPayment = t.TransactionTypeId switch
            {
                1 => "Cash Transaction",
                2 => "Prepaid Transaction - RFID Tag",
                3 => "Prepaid Transaction - Card",
                4 => "Exempt Transaction",
                5 => "Violation",
                6 => "Bank Card(NFC - Post Paid)",
                7 => "QR Code / Barcode",
                8 => "Convoy"
            },
   

            Classification = t.ActualTollClassId.HasValue ?
                 (t.ActualTollClassId.Value == 0 ? "Class 0" :
                  t.ActualTollClassId.Value == 1 ? "Class 1" :
                  t.ActualTollClassId.Value == 2 ? "Class 2" :
                  t.ActualTollClassId.Value == 3 ? "Class 3" :
                  t.ActualTollClassId.Value == 4 ? "Class M" :
                  t.ActualTollClassId.Value == 5 ? "Class 5" : "Unknown")
                 : "Unknown",

            Revenue = (decimal?)t.NettAmount ?? 0
        }).ToList();






        // Apply optional filter (paymentMethods)
        if (paymentMethods != null && paymentMethods.Any())
        {
            paymentClassData = paymentClassData
                .Where(x => paymentMethods.Contains(x.MethodOfPayment))
                .ToList();
        }

        // Aggregate by MethodOfPayment
        //var aggregated = paymentClassData
        //    .GroupBy(p => p.MethodOfPayment)
        //    .Select(g => new
        //    {
        //        MethodOfPayment = g.Key,
        //        ClassI_Count = g.Count(x => x.FinalClass == "Class I"),
        //        ClassII_Count = g.Count(x => x.FinalClass == "Class II"),
        //        ClassIII_Count = g.Count(x => x.FinalClass == "Class III"),
        //        ClassM_Count = g.Count(x => x.FinalClass == "Class M"),
        //        ClassI_Revenue = g.Where(x => x.FinalClass == "Class I").Sum(x => x.Revenue),
        //        ClassII_Revenue = g.Where(x => x.FinalClass == "Class II").Sum(x => x.Revenue),
        //        ClassIII_Revenue = g.Where(x => x.FinalClass == "Class III").Sum(x => x.Revenue),
        //        ClassM_Revenue = g.Where(x => x.FinalClass == "Class M").Sum(x => x.Revenue)
        //    })
        //    .ToList();

        var result = new List<ComprehensiveDto>();
        return paymentClassData;

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
