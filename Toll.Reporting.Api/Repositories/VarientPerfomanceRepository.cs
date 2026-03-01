using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.DTOs;
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

        public async Task<PagedResult<VarientPerformanceDto>> GetVarientPerformanceAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            int page = 1,
            int pageSize = 10)
        {
            // ==================== BASE QUERY ====================
            var baseQuery =
                from t in _context.Transactions
                join s in _context.Shifts on t.ShiftId equals s.ShiftId into shiftGroup
                from s in shiftGroup.DefaultIfEmpty()
                join su in _context.SystemUsers on t.SystemUserId equals su.SystemUserId into userGroup
                from su in userGroup.DefaultIfEmpty()
                join cc in _context.CollectorCashups on t.AllocatedToCollectorCashupId equals cc.CollectorCashupId into cashupGroup
                from cc in cashupGroup.DefaultIfEmpty()
                where t.TransactionDateTime >= startDate && t.TransactionDateTime <= endDate
                select new
                {
                    t.ShiftDate,
                    ShiftDescription = s.Description,
                    TollOperator = su.Username,
                    CashExpected = (double?)(t.ActualAmount ?? 0.0),
                    CashDeclared = (double?)(cc.TotalDeclared)
                };

            // ==================== FILTERS ====================
            if (operationalShift != null && operationalShift.Any())
                baseQuery = baseQuery.Where(x => operationalShift.Contains(x.ShiftDescription));
            if (tollOperators != null && tollOperators.Any())
                baseQuery = baseQuery.Where(x => tollOperators.Contains(x.TollOperator));

            var data = await baseQuery.AsNoTracking().ToListAsync();

            // ==================== GROUP BY OPERATOR (PER SHIFT) ====================
            var operatorGroups = data
                .GroupBy(x => new { x.ShiftDescription, x.TollOperator })
                .Select(g => new VarientPerformanceDto
                {
                    ShiftDate = g.First().ShiftDate,
                    ShiftDescription = g.Key.ShiftDescription ?? "-- None --",
                    TollOperator = g.Key.TollOperator ?? "-- None --",
                    NominalTariff = g.Sum(y => y.CashExpected ?? 0.0),
                    ActualAmount = g.Sum(y => y.CashDeclared ?? 0.0),
                    Difference = g.Sum(y => (y.CashDeclared ?? 0.0) - (y.CashExpected ?? 0.0)),
                    StartDate = startDate,
                    EndDate = endDate
                })
                .OrderBy(x => x.ShiftDescription)
                .ThenBy(x => x.TollOperator)
                .ToList();

            // ==================== SHIFT TOTALS ====================
            var withShiftTotals = new List<VarientPerformanceDto>();

            foreach (var shiftGroup in operatorGroups.GroupBy(g => g.ShiftDescription))
            {
                // Add operator-level rows first
                withShiftTotals.AddRange(shiftGroup);

                // Then add a total row for this shift
                withShiftTotals.Add(new VarientPerformanceDto
                {
                    ShiftDate = shiftGroup.First().ShiftDate,
                    ShiftDescription = $"{shiftGroup.Key?.ToUpper() ?? "-- NONE --"} TOTAL",
                    TollOperator = "—",
                    NominalTariff = shiftGroup.Sum(x => x.NominalTariff),
                    ActualAmount = shiftGroup.Sum(x => x.ActualAmount),
                    Difference = shiftGroup.Sum(x => x.Difference),
                    StartDate = startDate,
                    EndDate = endDate
                });
            }

            // ==================== GRAND TOTAL ====================
            if (withShiftTotals.Any())
            {
                var grandTotal = new VarientPerformanceDto
                {
                    ShiftDate = withShiftTotals.First().ShiftDate,
                    ShiftDescription = "GRAND TOTAL",
                    TollOperator = "—",
                    NominalTariff = withShiftTotals
                        .Where(x => x.ShiftDescription.EndsWith("TOTAL"))
                        .Sum(x => x.NominalTariff),
                    ActualAmount = withShiftTotals
                        .Where(x => x.ShiftDescription.EndsWith("TOTAL"))
                        .Sum(x => x.ActualAmount),
                    Difference = withShiftTotals
                        .Where(x => x.ShiftDescription.EndsWith("TOTAL"))
                        .Sum(x => x.Difference),
                    StartDate = startDate,
                    EndDate = endDate
                };

                withShiftTotals.Add(grandTotal);
            }

            // ==================== PAGINATION ====================
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

        // === Dropdown Helper Methods ===
        public async Task<IEnumerable<string>> GetShiftsAsync()
        {
            return await _context.Shifts
                .Select(s => s.ShiftId.ToString())
                .Where(s => s != null)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetTollOperatorsAsync()
        {
            return await _context.SystemUsers
                .Select(su => su.Username)
                .Where(su => su != null)
                .Distinct()
                .OrderBy(su => su)
                .ToListAsync();
        }
    }
}
