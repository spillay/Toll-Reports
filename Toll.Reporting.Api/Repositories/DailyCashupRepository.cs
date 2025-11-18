using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toll.Reporting.Api.DTOs;
using TollReportingSystem.Data;

namespace Toll.Reporting.Api.Repositories
{
    public class DailyCashupRepository : IDailyCashupRepository
    {
        private readonly ApplicationDbContext _context;

        public DailyCashupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Fetch paginated daily cashup records, grouped by Toll Operator + Shift,
        /// with sums per operator per shift.
        /// NettAmount  = sum of all NettAmount for the operator & shift (Lane Cash)
        /// ActualAmount = sum of ActualAmount (can be used for Top-ups if mapped that way)
        /// TotalDeclared = sum of Cash Declared from CollectorCashups
        /// Difference = (NettAmount + ActualAmount) - TotalDeclared
        /// </summary>
        public async Task<PagedResult<DailyCashupDto>> GetDailyCashupAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            int page = 1,
            int pageSize = 10)
        {
            // base query (per transaction)
            var baseQuery =
                from su in _context.SystemUsers
                join t in _context.Transactions
                    on su.SystemUserId equals t.SystemUserId into tGroup
                from t in tGroup.DefaultIfEmpty()

                join s in _context.Shifts
                    on t.ShiftId equals s.ShiftId into sGroup
                from s in sGroup.DefaultIfEmpty()

                join cc in _context.CollectorCashups
                    on new { su.SystemUserId, t.ShiftId }
                    equals new { cc.SystemUserId, cc.ShiftId } into ccGroup
                from cc in ccGroup.DefaultIfEmpty()

                where t.TransactionDateTime >= startDate && t.TransactionDateTime <= endDate
                select new
                {
                    ShiftDescription = s.Description,
                    TollOperator = su.Username,
                    NettAmount = (double?)t.NettAmount,      // system NettAmount
                    ActualAmount = (double?)t.ActualAmount,  // system ActualAmount (can be Top-up)
                    TotalDeclared = (double?)cc.TotalDeclared,
                    TransactionDate = (DateTime?)t.TransactionDateTime
                };

            // dynamic filters
            if (operationalShift != null && operationalShift.Any() && !operationalShift.Contains("-- All --"))
                baseQuery = baseQuery.Where(x => operationalShift.Contains(x.ShiftDescription));

            if (tollOperators != null && tollOperators.Any() && !tollOperators.Contains("-- All --"))
                baseQuery = baseQuery.Where(x => tollOperators.Contains(x.TollOperator));

            // group BY Toll Operator + Shift
            var groupedQuery = baseQuery
                .GroupBy(x => new { x.TollOperator, x.ShiftDescription })
                .Select(g => new DailyCashupDto
                {
                    ShiftDescription = g.Key.ShiftDescription ?? "-- None --",
                    TollOperator = g.Key.TollOperator ?? "-- None --",

                    // sums per operator + shift
                    NettAmount = g.Sum(x => x.NettAmount ?? 0d),        // Lane Cash total
                    ActualAmount = g.Sum(x => x.ActualAmount ?? 0d),    // Top-ups total
                    TotalDeclared = g.Sum(x => x.TotalDeclared ?? 0d),  // Declared total

                    // Expected = LaneCash + Top-ups, Difference = Expected - Declared
                    Difference = (g.Sum(x => x.NettAmount ?? 0d) + g.Sum(x => x.ActualAmount ?? 0d))
                                 - g.Sum(x => x.TotalDeclared ?? 0d),

                    // any date inside the group (min used as shift date)
                    ShiftDate = g.Min(x => x.TransactionDate) ?? startDate.Date
                });

            // count groups (not raw rows)
            var totalCount = await groupedQuery.CountAsync();

            // apply pagination on grouped result
            var pagedItems = await groupedQuery
                .OrderBy(x => x.ShiftDescription)
                .ThenBy(x => x.TollOperator)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new PagedResult<DailyCashupDto>
            {
                Items = pagedItems,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        /// <summary>
        /// Fetch filter options dynamically for dropdowns (Shifts, Operators)
        /// </summary>
        public async Task<DailyCashupFilterOptionsDto> GetDailyCashupFilterOptionsAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null)
        {
            var query =
                from su in _context.SystemUsers
                join t in _context.Transactions
                    on su.SystemUserId equals t.SystemUserId into tGroup
                from t in tGroup.DefaultIfEmpty()
                join s in _context.Shifts
                    on t.ShiftId equals s.ShiftId into sGroup
                from s in sGroup.DefaultIfEmpty()
                where t.TransactionDateTime >= startDate && t.TransactionDateTime <= endDate
                select new
                {
                    Shift = s.Description,
                    Operator = su.Username
                };

            if (operationalShift?.Any() == true && !operationalShift.Contains("-- All --"))
                query = query.Where(x => operationalShift.Contains(x.Shift));

            if (tollOperators?.Any() == true && !tollOperators.Contains("-- All --"))
                query = query.Where(x => tollOperators.Contains(x.Operator));

            var shifts = await query.Select(x => x.Shift)
                                    .Where(x => !string.IsNullOrEmpty(x))
                                    .Distinct()
                                    .OrderBy(x => x)
                                    .ToListAsync();

            var operators = await query.Select(x => x.Operator)
                                       .Where(x => !string.IsNullOrEmpty(x))
                                       .Distinct()
                                       .OrderBy(x => x)
                                       .ToListAsync();

            return new DailyCashupFilterOptionsDto
            {
                Shifts = shifts,
                TollOperators = operators
            };
        }

        public async Task<IEnumerable<string>> GetShiftsAsync()
        {
            return await _context.Shifts
                                 .Select(s => s.Description)
                                 .Distinct()
                                 .OrderBy(s => s)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetTollOperatorsAsync()
        {
            return await _context.SystemUsers
                                 .Select(su => su.Username)
                                 .Distinct()
                                 .OrderBy(su => su)
                                 .ToListAsync();
        }
    }
}
