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
        // ✅ MAIN QUERY
        var query =
            from t in _context.Transactions

            join s in _context.Shifts
                on (int?)t.ShiftId equals (int?)s.ShiftId into sGroup
            from s in sGroup.DefaultIfEmpty()

            join u in _context.SystemUsers
                on (long?)t.SystemUserId equals (long?)u.SystemUserId into uGroup
            from u in uGroup.DefaultIfEmpty()

            join l in _context.Lanes
                on (int?)t.LaneId equals (int?)l.LaneId into lGroup
            from l in lGroup.DefaultIfEmpty()

            join tt in _context.TransactionTypes
                on (int?)t.TransactionTypeId equals (int?)tt.TransactionTypeId into ttGroup
            from tt in ttGroup.DefaultIfEmpty()

            join d in _context.DiscountTypes
                on (int?)t.DiscountTypeId equals (int?)d.DiscountTypeId into dGroup
            from d in dGroup.DefaultIfEmpty()

            join tc in _context.TollClasses
                on (int?)t.ManualTollClassId equals (int?)tc.TollClassId into tcGroup
            from tc in tcGroup.DefaultIfEmpty()

            join tp in _context.TariffPlans
                on (int?)t.TariffPlanId equals (int?)tp.TariffPlanId into tpGroup
            from tp in tpGroup.DefaultIfEmpty()

            join tpd in _context.TariffPlanDetails
    on new
    {
        TariffPlanId = (int?)t.TariffPlanId,
        TollClassId = (int?)t.ManualTollClassId,
        TransactionTypeId = (int?)t.TransactionTypeId
    }
    equals new
    {
        TariffPlanId = (int?)tpd.TariffPlanId,
        TollClassId = (int?)tpd.TollClassId,
        TransactionTypeId = (int?)tpd.TransactionTypeId
    } into tpdGroup
            from tpd in tpdGroup.DefaultIfEmpty()


            where t.TransactionDateTime >= startDate && t.TransactionDateTime <= endDate

            select new
            {
                t.TransactionDateTime,
                Shift = s.Description,
                TollOperator = u.Username, 
                LaneName = l.LaneName,
                TransactionType = tt.Description,
                DiscountType = d.Description,
                ManualClass = tc.ClassDescription,
                TariffPlanId = t.TariffPlanId,
                MethodOfPayment = tt.Description, 
                AmountInclusive = tpd.AmountInclusive
            };

        var raw = await query.AsNoTracking().ToListAsync();

        // Helper: Convert filters to lowercase sets
        static HashSet<string>? ToSet(List<string>? list) =>
            list?.Select(s => s.Trim().ToLowerInvariant()).ToHashSet();

        var shiftSet = ToSet(operationalShift);
        var opSet = ToSet(tollOperators);
        var laneSet = ToSet(laneNames);
        var discSet = ToSet(laneDiscountTypes);
        var classSet = ToSet(classification);
        var paySet = ToSet(paymentMethods);
        var trxTypeSet = ToSet(transactionTypes);

        static bool Match(string? value, HashSet<string>? set) =>
            set == null || (value != null && set.Contains(value.Trim().ToLowerInvariant()));

        // ✅ Filter + Map to DTO
        var result = raw
            .Where(x =>
                Match(x.Shift, shiftSet) &&
                Match(x.TollOperator, opSet) &&
                Match(x.LaneName, laneSet) &&
                Match(x.DiscountType, discSet) &&
                Match(x.ManualClass, classSet) &&
                Match(x.MethodOfPayment, paySet) &&
                Match(x.TransactionType, trxTypeSet)
            )
            .Select(x => new ComprehensiveDto
            {
                LaneName = x.LaneName,
                TransactionType = x.TransactionType,
                DiscountType = x.DiscountType,
                TransactionDateTime = x.TransactionDateTime,
                Shift = x.Shift,
                ManualTollClass = x.ManualClass,
                TariffPlanId = x.TariffPlanId,
                AmountInclusive = x.AmountInclusive,
                MethodOfPayment = x.MethodOfPayment,
                TollOperatorID = x.TollOperator // ✅ Map username to TollOperatorID property
            })
            .OrderByDescending(x => x.TransactionDateTime)
            .ToList();

        return result;
    }
}
