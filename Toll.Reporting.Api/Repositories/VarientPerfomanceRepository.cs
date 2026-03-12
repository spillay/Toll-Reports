using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.DTOs;
using Toll.Reporting.Api.Repositories.Interfaces;
using TollReportingSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                .Distinct()
                .ToList();

            return normalized.Count == 0 ? null : normalized;
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

            // =========================================================
            // 1. BASE TRANSACTION QUERY
            //    This is the real source for toll operator totals.
            // =========================================================
            var transactionQuery =
                from t in _context.Transactions
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
                    CashExpected = (double?)(t.NominalTariff),
                    t.AllocatedToCollectorCashupId
                };

            // =========================================================
            // 2. APPLY FILTERS
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

            var transactionData = await transactionQuery
                .AsNoTracking()
                .ToListAsync();

            // No records, return empty result early
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
            //    This is based ONLY on transactions.
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

                    // Keep distinct cashup ids linked to this operator's transactions
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
            //    Important: each cashup must only be counted once.
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
                    cc => (double?)(cc.TotalDeclared))
            : new Dictionary<long, double?>();  

            // =========================================================
            // 5. BUILD FINAL OPERATOR ROWS
            //    Declared amount is sum of DISTINCT related cashups only.
            // =========================================================
            var operatorRows = expectedByOperator
                .Select(x =>
                {
                    var actualAmount = x.CashupIds
                        .Distinct()
                        .Sum(cashupId => declaredCashupMap.TryGetValue(cashupId, out var declared)
                            ? (declared ?? 0.0)
                            : 0.0);

                    return new VarientPerformanceDto
                    {
                        ShiftDate = x.ShiftDate,
                        ShiftDescription = x.ShiftDescription,
                        TollOperator = x.TollOperator,
                        NominalTariff = x.NominalTariff,
                        ActualAmount = actualAmount,
                        Difference = actualAmount - x.NominalTariff,
                        StartDate = startDate,
                        EndDate = endDate
                    };
                })
                .ToList();

            // =========================================================
            // 6. ADD SHIFT TOTALS
            //    Totals are based on the already-correct operator rows.
            // =========================================================
            var withShiftTotals = new List<VarientPerformanceDto>();

            foreach (var shiftGroup in operatorRows
                .GroupBy(x => x.ShiftDescription)
                .OrderBy(g => g.Key))
            {
                var rows = shiftGroup.ToList();

                // Add detail rows first
                withShiftTotals.AddRange(rows);

                // Add shift total row
                withShiftTotals.Add(new VarientPerformanceDto
                {
                    ShiftDate = rows.First().ShiftDate,
                    ShiftDescription = $"{shiftGroup.Key.ToUpper()} TOTAL",
                    TollOperator = "—",
                    NominalTariff = rows.Sum(x => x.NominalTariff),
                    ActualAmount = rows.Sum(x => x.ActualAmount),
                    Difference = rows.Sum(x => x.Difference),
                    StartDate = startDate,
                    EndDate = endDate
                });
            }

            // =========================================================
            // 7. ADD GRAND TOTAL
            //    Only sum shift total rows, not detail rows again.
            // =========================================================
            var shiftTotals = withShiftTotals
                .Where(x => x.ShiftDescription.EndsWith("TOTAL")
                         && x.ShiftDescription != "GRAND TOTAL")
                .ToList();

            if (shiftTotals.Any())
            {
                withShiftTotals.Add(new VarientPerformanceDto
                {
                    ShiftDate = shiftTotals.First().ShiftDate,
                    ShiftDescription = "GRAND TOTAL",
                    TollOperator = "—",
                    NominalTariff = shiftTotals.Sum(x => x.NominalTariff),
                    ActualAmount = shiftTotals.Sum(x => x.ActualAmount),
                    Difference = shiftTotals.Sum(x => x.Difference),
                    StartDate = startDate,
                    EndDate = endDate
                });
            }

            // =========================================================
            // 8. PAGINATION
            // =========================================================
            var totalCount = withShiftTotals.Count;

            var pagedItems = withShiftTotals
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

        // =========================================================
        // FILTER OPTIONS
        // Use descriptions / usernames, not IDs, since your filters
        // are matching display values in the UI.
        // =========================================================
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