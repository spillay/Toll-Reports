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
        private const byte CashTransactionTypeId = 1;
        public async Task<PagedResult<DailyCashupDto>> GetDailyCashupAsync(
        DateTime startDate,
        DateTime endDate,
        List<int>? shiftIds = null,
        List<long>? systemUserIds = null,
        int page = 1,
        int pageSize = 10)
        {
                var start = startDate.Date;
                var end = endDate.Date.AddDays(1).AddTicks(-1);


            // 1) Transactions base (ONLY rows with operator)
            var txBase =
            from t in _context.Transactions
            where t.TransactionDateTime >= start
                  && t.TransactionDateTime <= end
                  && t.SystemUserId != null
                  && t.TransactionTypeId == CashTransactionTypeId
                  && (shiftIds == null || shiftIds.Count == 0 || shiftIds.Contains((int)t.ShiftId))
                  && (systemUserIds == null || systemUserIds.Count == 0 || systemUserIds.Contains(t.SystemUserId.Value))
            select new
            {
                SystemUserId = t.SystemUserId.Value,
                ShiftId = (int)t.ShiftId,
                ShiftDate = t.ShiftDate.Date,
                NettAmount = t.NettAmount,
                ActualAmount = (double?)t.ActualAmount ?? 0d
            };

            // 2) Lookup joins (SystemUser + Shift)
            var txWithNames =
                    from t in txBase
                    join su in _context.SystemUsers on t.SystemUserId equals (long)su.SystemUserId
                    join sh in _context.Shifts on (int)t.ShiftId equals (int)sh.ShiftId into shGroup
                    from sh in shGroup.DefaultIfEmpty()
                    select new
                    {
                        t.SystemUserId,
                        t.ShiftId,
                        t.ShiftDate,
                        ShiftDescription = sh != null ? sh.Description : null,
                        TollOperator = su.Username,
                        t.NettAmount,
                        t.ActualAmount
                    };

            // 3) Multi-select filters (same filters, now multi)
            if (shiftIds != null && shiftIds.Any())
                txWithNames = txWithNames.Where(x => shiftIds.Contains(x.ShiftId));

            if (systemUserIds != null && systemUserIds.Any())
                txWithNames = txWithNames.Where(x => systemUserIds.Contains(x.SystemUserId));

            // 4) Aggregate transactions to DAILY grain
            var txAgg =
                    from x in txWithNames
                    group x by new
                    {
                        x.SystemUserId,
                        x.ShiftId,
                        x.ShiftDate,
                        x.ShiftDescription,
                        x.TollOperator
                    }
                    into g
                    select new
                    {
                        g.Key.SystemUserId,
                        g.Key.ShiftId,
                        g.Key.ShiftDate,
                        g.Key.ShiftDescription,
                        g.Key.TollOperator,
                        NettAmount = g.Sum(v => v.NettAmount),
                        ActualAmount = g.Sum(v => v.ActualAmount)
                    };

                // 5) Cashup aggregate (DAILY grain)
                var cashupAgg =
                    from cc in _context.CollectorCashups
                    where cc.ShiftDate >= start && cc.ShiftDate <= end
                    group cc by new
                    {
                        cc.SystemUserId,
                        cc.ShiftId,
                        ShiftDate = cc.ShiftDate.Date
                    }
                    into g
                    select new
                    {
                        g.Key.SystemUserId,
                        g.Key.ShiftId,
                        g.Key.ShiftDate,
                        TotalDeclared = g.Sum(x => x.TotalDeclared)    
                    };

                // 6) SAFE declared value via correlated subquery
                var finalQuery =
                    from t in txAgg
                    let declared =
                        cashupAgg
                            .Where(c =>
                                c.SystemUserId == t.SystemUserId &&
                                c.ShiftId == t.ShiftId &&
                                c.ShiftDate == t.ShiftDate)
                            .Select(c => (double?)c.TotalDeclared)
                            .FirstOrDefault() ?? 0d
                    select new DailyCashupDto
                    {
                        ShiftDate = t.ShiftDate,
                        ShiftDescription = t.ShiftDescription ?? "-- None --",
                        TollOperator = t.TollOperator ?? "-- None --",
                        NettAmount = t.NettAmount,
                        ActualAmount = t.ActualAmount,
                        TotalDeclared = declared,
                        Difference = (t.NettAmount + t.ActualAmount) - declared
                    };

                // 7) Pagination
                var totalCount = await finalQuery.CountAsync();

                var items = await finalQuery
                    .OrderBy(x => x.ShiftDate)
                    .ThenBy(x => x.ShiftDescription)
                    .ThenBy(x => x.TollOperator)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                return new PagedResult<DailyCashupDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = totalPages
                };
            }

        public async Task<DailyCashupFilterOptionsDto> GetDailyCashupFilterOptionsAsync()
        {
            var shifts = await _context.Shifts
                .Select(s => new FilterItemDto<int>
                {
                    Id = (int)s.ShiftId,
                    Name = s.Description ?? ""
                })
                .Where(x => x.Name != "")
                .OrderBy(x => x.Name)
                .ToListAsync();

            var operators = await _context.SystemUsers
                .Select(su => new FilterItemDto<long>
                {
                    Id = (long)su.SystemUserId,
                    Name = su.Username ?? ""
                })
                .Where(x => x.Name != "")
                .OrderBy(x => x.Name)
                .ToListAsync();

            return new DailyCashupFilterOptionsDto
            {
                Shifts = shifts,
                TollOperators = operators
            };
        }
    }
}