using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.DTOs;
using TollReportingSystem.Data;

namespace Toll.Reporting.Api.Repositories
{
    public class VarientPerformanceRepository : IVarientPerformanceRepository
    {
        private readonly ApplicationDbContext _context;

        public VarientPerformanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private static List<string>? NormalizeList(List<string>? values)
        {
            if (values == null) return null;

            var normalized = values
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Select(v => v.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return normalized.Count == 0 ? null : normalized;
        }

        private static DateTime NormalizeEndDate(DateTime endDate)
        {
            return (endDate.TimeOfDay == TimeSpan.Zero)
                ? endDate.Date.AddDays(1).AddSeconds(-1)
                : endDate;
        }

        public async Task<PagedResult<VarientPerformanceDto>> GetVarientPerformanceAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            int page = 1,
            int pageSize = 10)
        {
            operationalShift = NormalizeList(operationalShift);
            tollOperators = NormalizeList(tollOperators);
            endDate = NormalizeEndDate(endDate);

            // =========================================================
            // 1. BASE TRANSACTION QUERY FOR STAFF PERFORMANCE
            // =========================================================
            var transactionQuery =
                from t in _context.Transactions.AsNoTracking()
                join s in _context.Shifts on t.ShiftId equals s.ShiftId into shiftGroup
                from s in shiftGroup.DefaultIfEmpty()
                join su in _context.SystemUsers on t.SystemUserId equals su.SystemUserId into userGroup
                from su in userGroup.DefaultIfEmpty()
                where t.TransactionDateTime >= startDate
                   && t.TransactionDateTime <= endDate
                select new
                {
                    t.TransactionNumber,
                    t.ShiftDate,
                    t.ShiftId,
                    ShiftDescription = s.Description,
                    t.SystemUserId,
                    TollOperator = su.Username,
                    CashExpected = (double?)t.NominalTariff,
                    t.AllocatedToCollectorCashupId
                };

            // =========================================================
            // 2. APPLY STAFF FILTERS
            // =========================================================
            if (operationalShift != null && operationalShift.Any())
            {
                transactionQuery = transactionQuery
                    .Where(x => x.ShiftDescription != null && operationalShift.Contains(x.ShiftDescription));
            }

            if (tollOperators != null && tollOperators.Any())
            {
                transactionQuery = transactionQuery
                    .Where(x => x.TollOperator != null && tollOperators.Contains(x.TollOperator));
            }

            var transactionData = await transactionQuery.ToListAsync();

            if (!transactionData.Any())
            {
                return new PagedResult<VarientPerformanceDto>
                {
                    Items = new List<VarientPerformanceDto>(),
                    TotalCount = 0,
                    Page = page,
                    PageSize = pageSize
                };
            }

            // =========================================================
            // 3. GROUP EXPECTED CASH BY SHIFT + OPERATOR
            // =========================================================
            var expectedByOperator = transactionData
                .GroupBy(x => new
                {
                    x.ShiftDate,
                    x.ShiftId,
                    x.ShiftDescription,
                    x.SystemUserId,
                    x.TollOperator
                })
                .Select(g => new
                {
                    g.Key.ShiftDate,
                    g.Key.ShiftId,
                    ShiftDescription = g.Key.ShiftDescription ?? "-- None --",
                    g.Key.SystemUserId,
                    TollOperator = g.Key.TollOperator ?? "-- None --",
                    NominalTariff = g.Sum(x => x.CashExpected ?? 0.0),

                    CashupIds = g.Where(x => x.AllocatedToCollectorCashupId != null)
                                 .Select(x => x.AllocatedToCollectorCashupId!.Value)
                                 .Distinct()
                                 .ToList()
                })
                .OrderBy(x => x.ShiftDescription)
                .ThenBy(x => x.TollOperator)
                .ToList();

            // =========================================================
            // 4. FETCH DECLARED CASHUPS ONCE
            // =========================================================
            var allCashupIds = expectedByOperator
                .SelectMany(x => x.CashupIds)
                .Distinct()
                .ToList();

            var declaredCashupMap = allCashupIds.Any()
                ? await _context.CollectorCashups
                    .AsNoTracking()
                    .Where(cc => allCashupIds.Contains(cc.CollectorCashupId))
                    .ToDictionaryAsync(
                        cc => cc.CollectorCashupId,
                        cc => (double?)cc.TotalDeclared)
                : new Dictionary<long, double?>();

            // =========================================================
            // 5. BUILD DISCREPANCY TOTALS BY SHIFT + OPERATOR
            //    This mirrors the discrepancy report logic enough for totals.
            // =========================================================
            var discrepancyBaseQuery =
                from t in _context.Transactions.AsNoTracking()
                join s in _context.Shifts on t.ShiftId equals s.ShiftId into shiftGroup
                from s in shiftGroup.DefaultIfEmpty()

                join su in _context.SystemUsers on t.SystemUserId equals su.SystemUserId into userGroup
                from su in userGroup.DefaultIfEmpty()

                join tc1 in _context.TollClasses on t.ManualTollClassId equals tc1.TollClassId into tc1Group
                from tc1 in tc1Group.DefaultIfEmpty()

                join tc2 in _context.TollClasses on t.AutomaticTollClassId equals tc2.TollClassId into tc2Group
                from tc2 in tc2Group.DefaultIfEmpty()

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

                where t.TransactionDateTime >= startDate
                   && t.TransactionDateTime <= endDate
                select new
                {
                    t.ShiftDate,
                    ShiftDescription = s.Description,
                    TollOperator = su.Username,
                    ManualClass = tc1.ClassDescription,
                    AutomaticClass = tc2.ClassDescription,
                    AmountInclusive = (double?)tpd.AmountInclusive,
                    AmountExclusive = (double?)tpd.AmountExclusive
                };

            // Same filters as staff report so totals line up
            if (operationalShift != null && operationalShift.Any())
            {
                discrepancyBaseQuery = discrepancyBaseQuery
                    .Where(x => x.ShiftDescription != null && operationalShift.Contains(x.ShiftDescription));
            }

            if (tollOperators != null && tollOperators.Any())
            {
                discrepancyBaseQuery = discrepancyBaseQuery
                    .Where(x => x.TollOperator != null && tollOperators.Contains(x.TollOperator));
            }

            // Only true discrepancies
            discrepancyBaseQuery = discrepancyBaseQuery.Where(x =>
                (x.ManualClass ?? "").Trim() != (x.AutomaticClass ?? "").Trim());

            var discrepancyTotals = await discrepancyBaseQuery
                .GroupBy(x => new
                {
                    x.ShiftDate,
                    ShiftDescription = x.ShiftDescription ?? "-- None --",
                    TollOperator = x.TollOperator ?? "-- None --"
                })
                .Select(g => new
                {
                    g.Key.ShiftDate,
                    g.Key.ShiftDescription,
                    g.Key.TollOperator,

                    // Use the same difference direction your discrepancy report uses.
                    // If your report calculates it the other way around, flip this line.
                    DiscrepancyDifference = g.Sum(x =>
                        (x.AmountExclusive ?? 0.0) - (x.AmountInclusive ?? 0.0))
                })
                .ToListAsync();

            var discrepancyLookup = discrepancyTotals.ToDictionary(
                x => $"{x.ShiftDate:yyyy-MM-dd}|||{x.ShiftDescription}|||{x.TollOperator}",
                x => x.DiscrepancyDifference,
                StringComparer.OrdinalIgnoreCase);

            // =========================================================
            // 6. BUILD FINAL OPERATOR ROWS
            // =========================================================
            var operatorRows = expectedByOperator
                .Select(x =>
                {
                    var actualAmount = x.CashupIds
                        .Distinct()
                        .Sum(cashupId => declaredCashupMap.TryGetValue(cashupId, out var declared)
                            ? (declared ?? 0.0)
                            : 0.0);

                    var discrepancyKey = $"{x.ShiftDate:yyyy-MM-dd}|||{x.ShiftDescription}|||{x.TollOperator}";
                    var discrepancyDifference = discrepancyLookup.TryGetValue(discrepancyKey, out var totalDiscrepancy)
                        ? totalDiscrepancy
                        : 0.0;

                    return new VarientPerformanceDto
                    {
                        ShiftDate = x.ShiftDate,
                        ShiftDescription = x.ShiftDescription,
                        TollOperator = x.TollOperator,
                        NominalTariff = x.NominalTariff,
                        ActualAmount = actualAmount,
                        Difference = actualAmount - x.NominalTariff,
                        DiscrepancyDifference = discrepancyDifference,
                        StartDate = startDate,
                        EndDate = endDate
                    };
                })
                .ToList();

            // =========================================================
            // 7. PAGINATION
            // =========================================================
            var totalCount = operatorRows.Count;

            var pagedItems = operatorRows
                .OrderBy(x => x.ShiftDate)
                .ThenBy(x => x.ShiftDescription)
                .ThenBy(x => x.TollOperator)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<VarientPerformanceDto>
            {
                Items = pagedItems,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<string>> GetShiftsAsync()
        {
            return await _context.Shifts
                .AsNoTracking()
                .Where(s => s.Description != null && s.Description != "")
                .Select(s => s.Description!)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetTollOperatorsAsync()
        {
            return await _context.SystemUsers
                .AsNoTracking()
                .Where(su => su.Username != null && su.Username != "")
                .Select(su => su.Username!)
                .Distinct()
                .OrderBy(su => su)
                .ToListAsync();
        }
    }
}
