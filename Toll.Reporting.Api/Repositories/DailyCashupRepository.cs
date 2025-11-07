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
        /// Fetch paginated daily cashup records (variance between system totals and declared cash)
        /// </summary>
        public async Task<PagedResult<DailyCashupDto>> GetDailyCashupAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            int page = 1,
            int pageSize = 10)
        {
            var query =
                from su in _context.SystemUsers
                join t in _context.Transactions
                    on su.SystemUserId equals t.SystemUserId into tGroup
                from t in tGroup.DefaultIfEmpty()

                join s in _context.Shifts
                    on t.ShiftId equals s.ShiftId into sGroup
                from s in sGroup.DefaultIfEmpty()

                join cc in _context.CollectorCashups
                    on su.SystemUserId equals cc.SystemUserId into ccGroup
                from cc in ccGroup.DefaultIfEmpty()

                where t.TransactionDateTime >= startDate && t.TransactionDateTime <= endDate
                select new
                {
                    ShiftDescription = s.Description,
                    TollOperator = su.Username,
                    NettAmount = (double?)t.NettAmount,
                    ActualAmount = (double?)t.ActualAmount,
                    TotalDeclared = (double?)cc.TotalDeclared,
                    TransactionDate = (DateTime?)t.TransactionDateTime
                };


            // Apply filters dynamically
            if (operationalShift != null && operationalShift.Any() && !operationalShift.Contains("-- All --"))
                query = query.Where(x => operationalShift.Contains(x.ShiftDescription));

            if (tollOperators != null && tollOperators.Any() && !tollOperators.Contains("-- All --"))
                query = query.Where(x => tollOperators.Contains(x.TollOperator));

            // Count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var pagedItems = await query
                .OrderByDescending(x => x.TransactionDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new DailyCashupDto
                {
                    ShiftDescription = x.ShiftDescription ?? "-- None --",
                    TollOperator = x.TollOperator ?? "-- None --",
                    NettAmount = x.NettAmount == null ? 0d : x.NettAmount.Value,
                    ActualAmount = x.ActualAmount == null ? 0d : x.ActualAmount.Value,
                    TotalDeclared = x.TotalDeclared == null ? 0d : x.TotalDeclared.Value,
                    Difference = (x.NettAmount ?? 0d) - (x.TotalDeclared ?? 0d),
                    ShiftDate = x.TransactionDate == null ? DateTime.MinValue : x.TransactionDate.Value.Date
                })
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

            // Apply filters dynamically (if pre-filtering required)
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
