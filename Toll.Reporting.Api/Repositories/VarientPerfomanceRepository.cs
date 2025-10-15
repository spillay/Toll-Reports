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

        /// <summary>
        /// Fetch paginated variant performance records with optional filters.
        /// </summary>
        public async Task<PagedResult<VarientPerformanceDto>> GetVarientPerformanceAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            int page = 1,
            int pageSize = 10)
        {
            // Base query joining only what's needed
            var query = from t in _context.Transactions
                        join s in _context.Shifts on t.ShiftId equals s.ShiftId into shiftGroup
                        from s in shiftGroup.DefaultIfEmpty()
                        join su in _context.SystemUsers on t.SystemUserId equals su.SystemUserId into userGroup
                        from su in userGroup.DefaultIfEmpty()
                        where t.TransactionDateTime >= startDate &&
                              t.TransactionDateTime < endDate.AddDays(1)
                        orderby t.TransactionDateTime descending
                        select new
                        {
                            t.ShiftDate,
                            Shift = s,
                            User = su,
                            t.ActualAmount,
                            t.NominalTariff
                        };

            // ✅ Optional filters
            if (operationalShift != null && operationalShift.Any() && !operationalShift.Contains("-- All --"))
            {
                query = query.Where(x => operationalShift.Contains(x.Shift.Description));
            }

            if (tollOperators != null && tollOperators.Any() && !tollOperators.Contains("-- All --"))
            {
                query = query.Where(x => tollOperators.Contains(x.User.Username));
            }

            // ✅ Count before pagination
            var totalCount = await query.CountAsync();

            // ✅ Apply pagination and map to DTO
            var pagedItems = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new VarientPerformanceDto
                {
                    ShiftDate = x.ShiftDate,
                    ShiftDescription = x.Shift.Description ?? "-- None --",
                    TollOperator = x.User.Username ?? "-- None --",
                    ActualAmount = x.ActualAmount ,
                    NominalTariff = x.NominalTariff ,
                    StartDate = startDate,
                    EndDate = endDate
                })
                .ToListAsync();

            return new PagedResult<VarientPerformanceDto>
            {
                Items = pagedItems,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        // ==================== LOOKUP QUERIES ====================

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
