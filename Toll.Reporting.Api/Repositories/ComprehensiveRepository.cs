using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toll.Reporting.Api.DTOs;
using TollReportingSystem.Data;

public class ComprehensiveRepository : IComprehensiveRepository
{
    private readonly ApplicationDbContext _context;

    public ComprehensiveRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Main repository method that supports multiple optional filters.
    /// All filters are treated as "include only if present" and compared by string equality.
    /// </summary>
    public async Task<IEnumerable<ComprehensiveDto>> GetComprehensiveRepositoryAsync(
        DateTime startDate,
        DateTime endDate,
        List<string>? operationalShift = null,
        List<string>? tollOperators = null,
        List<string>? laneNames = null,
        List<string>? laneDiscountTypes = null,
        List<string>? classification = null,
        List<string>? paymentMethods = null,
        List<string>? transactionTypes = null)
    {
        var result = new List<ComprehensiveDto>();

        try
        {
            // 1) Get transactions in date range
            var transactions = await _context.Transactions
                .Where(t => t.TransactionDateTime >= startDate && t.TransactionDateTime <= endDate)
                .ToListAsync();

            // 2) Load lookup tables once for fast mapping
            var laneDict = (await _context.Lanes.ToListAsync()).ToDictionary(l => l.LaneId, l => l.LaneName);
            var transTypeDict = (await _context.TransactionTypes.ToListAsync()).ToDictionary(t => t.TransactionTypeId, t => t.Description);
            var discountDict = (await _context.DiscountTypes.ToListAsync()).ToDictionary(d => d.DiscountTypeId, d => d.Description);
            var shiftDict = (await _context.Shifts.ToListAsync()).ToDictionary(s => s.ShiftId, s => s.Description);
            var userDict = (await _context.SystemUsers.ToListAsync()).ToDictionary(u => u.SystemUserId, u => u.Username);
            var tariffPlanDict = (await _context.TariffPlans.ToListAsync()).ToDictionary(tp => tp.TariffPlanId);
            var tariffPlanDetailLookup = (await _context.TariffPlanDetails.ToListAsync()).ToLookup(tpd => tpd.TariffPlanId);
            var tollClassDict = (await _context.TollClasses.ToListAsync()).ToDictionary(tc => tc.TollClassId, tc => tc.ClassDescription);

            // 3) Normalize filter collections to HashSet for O(1) contains checks (case-insensitive)
            HashSet<string>? shiftsFilter = operationalShift?.Select(s => s.Trim().ToLowerInvariant()).ToHashSet();
            HashSet<string>? tollOpsFilter = tollOperators?.Select(s => s.Trim().ToLowerInvariant()).ToHashSet();
            HashSet<string>? lanesFilter = laneNames?.Select(s => s.Trim().ToLowerInvariant()).ToHashSet();
            HashSet<string>? discountFilter = laneDiscountTypes?.Select(s => s.Trim().ToLowerInvariant()).ToHashSet();
            HashSet<string>? classFilter = classification?.Select(s => s.Trim().ToLowerInvariant()).ToHashSet();
            HashSet<string>? paymentFilter = paymentMethods?.Select(s => s.Trim().ToLowerInvariant()).ToHashSet();
            HashSet<string>? transTypeFilter = transactionTypes?.Select(s => s.Trim().ToLowerInvariant()).ToHashSet();

            int count = 0;
            foreach (var t in transactions)
            {
                // Map foreign key lookups (safe TryGet)
                laneDict.TryGetValue(t.LaneId, out var laneName);
                transTypeDict.TryGetValue(t.TransactionTypeId, out var transType);
                discountDict.TryGetValue(t.DiscountTypeId, out var discountType);
                shiftDict.TryGetValue(t.ShiftId, out var shift);
                userDict.TryGetValue((long)t.SystemUserId, out var username);
                tariffPlanDict.TryGetValue(t.TariffPlanId, out var tp);
                tollClassDict.TryGetValue(t.ManualTollClassId, out var manualClass);

                // Prepare fields for filtering (lowercase safe)
                string laneNameLower = laneName?.Trim().ToLowerInvariant() ?? string.Empty;
                string shiftLower = shift?.Trim().ToLowerInvariant() ?? string.Empty;
                string usernameLower = username?.Trim().ToLowerInvariant() ?? string.Empty;
                string discountLower = discountType?.Trim().ToLowerInvariant() ?? string.Empty;
                string classLower = manualClass?.Trim().ToLowerInvariant() ?? string.Empty;
                string transTypeLower = transType?.Trim().ToLowerInvariant() ?? string.Empty;

                // Attempt to get method of payment from transaction if property exists
                // NOTE: assumes your Transaction entity has MethodOfPayment or PaymentMethod string property.
                string methodOfPaymentValue = string.Empty;
                var methodProp = t.GetType().GetProperty("MethodOfPayment") ?? t.GetType().GetProperty("PaymentMethod");
                if (methodProp != null)
                {
                    var value = methodProp.GetValue(t);
                    methodOfPaymentValue = value?.ToString()?.Trim().ToLowerInvariant() ?? string.Empty;
                }

                // Apply filters (if any provided) — if any filter does not match, skip this transaction.
                if (shiftsFilter != null && !shiftsFilter.Contains(shiftLower))
                    continue;

                // tollOperators may be numeric ids or usernames; try both:
                if (tollOpsFilter != null)
                {
                    bool match = false;
                    // match by username
                    if (!string.IsNullOrEmpty(usernameLower) && tollOpsFilter.Contains(usernameLower))
                        match = true;

                    // match by id (string), try parse
                    if (!match && tollOpsFilter.Any(s => long.TryParse(s, out long opId) && opId == t.SystemUserId))
                        match = true;

                    if (!match)
                        continue;
                }

                if (lanesFilter != null && !lanesFilter.Contains(laneNameLower))
                    continue;

                if (paymentFilter != null && !paymentFilter.Contains(methodOfPaymentValue))
                    continue;

                if (discountFilter != null && !discountFilter.Contains(discountLower))
                    continue;

                if (classFilter != null && !classFilter.Contains(classLower))
                    continue;

                if (transTypeFilter != null && !transTypeFilter.Contains(transTypeLower))
                    continue;

                // Get the tariff plan details relevant to this tariff plan
                var details = tariffPlanDetailLookup[t.TariffPlanId];

                foreach (var d in details)
                {
                    var dto = new ComprehensiveDto
                    {
                        LaneName = laneName,
                        TransactionType = transType,
                        DiscountType = discountType,
                        TransactionDateTime = t.TransactionDateTime,
                        Shift = shift,
                        ManualTollClass = manualClass,
                        TariffPlanId = tp?.TariffPlanId ?? 0,
                        AmountInclusive = d.AmountInclusive,
                        MethodOfPayment = methodOfPaymentValue,
                        DiscountTypeId = t.DiscountTypeId,
                        SystemUserId = t.SystemUserId,
                        ManualTollClassId = t.ManualTollClassId,
                        TariffPlanDetailId = d.TariffPlanId
                    };

                    result.Add(dto);
                }

                if (count++ >= 10_000) break; // safety cap — adjust/remove for production
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            // do not rethrow; return what we have (or you may choose to throw)
        }

        return result;
    }

    // Keep the old not-implemented variant only if you need it elsewhere - otherwise remove it
    //public Task<IEnumerable<ComprehensiveDto>> GetComprehensiveRepositoryAsync1(DateTime startDate, DateTime endDate, List<string>? operationalShift = null, List<string>? tollOperators = null, List<string>? laneNames = null, List<string>? laneDiscountType = null, List<string>? Classification = null, List<string>? paymentMethods = null)
    //{
    //    throw new NotImplementedException();
    //}
}
