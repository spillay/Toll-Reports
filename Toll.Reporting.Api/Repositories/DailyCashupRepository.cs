using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Toll.Reporting.Api.DTOs;
using Toll.Reporting.Api.Repositories.Interfaces;
using TollReportingSystem.Data;

namespace Toll.Reporting.Api.Repositories
{
    public class DailyCashupRepository : IDailyCashupRepository
    {
        private readonly ApplicationDbContext _context;
        private const byte CashTransactionTypeId = 1;

        public DailyCashupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private sealed class DailyCashupTopupAggRow
        {
            public long SystemUserId { get; set; }
            public int ShiftId { get; set; }
            public DateTime ShiftDate { get; set; }
            public decimal ActualAmount { get; set; }
        }

        private sealed class DailyCashupBaseKey
        {
            public long SystemUserId { get; set; }
            public int ShiftId { get; set; }
            public DateTime ShiftDate { get; set; }
        }

        /// <summary>
        /// Gets cash-only top-up aggregates from RegisteredUserTopUp
        /// </summary>
        private async Task<List<DailyCashupTopupAggRow>> GetCashTopupAggregatesAsync(
            DateTime start,
            DateTime end,
            List<int>? shiftIds,
            List<long>? systemUserIds)
        {
            var results = new List<DailyCashupTopupAggRow>();

            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            await using var command = connection.CreateCommand();

            command.CommandText = @"
                SELECT
                    CAST(rut.SystemUserId AS bigint) AS SystemUserId,
                    CAST(rut.RechargeShift AS int) AS ShiftId,
                    CAST(rut.RechargedOn AS date) AS ShiftDate,
                    SUM(CAST(rut.Amount AS decimal(18,2))) AS ActualAmount
                FROM RegisteredUserTopUp rut
                INNER JOIN PaymentMethod pm
                    ON rut.PaymentMethodId = pm.PaymentMethodId
                WHERE rut.RechargedOn >= @start
                  AND rut.RechargedOn <= @end
                  AND rut.SystemUserId IS NOT NULL
                  AND LOWER(LTRIM(RTRIM(pm.Description))) = 'cash'
                GROUP BY
                    CAST(rut.SystemUserId AS bigint),
                    CAST(rut.RechargeShift AS int),
                    CAST(rut.RechargedOn AS date)
                ORDER BY
                    CAST(rut.RechargedOn AS date),
                    CAST(rut.RechargeShift AS int),
                    CAST(rut.SystemUserId AS bigint);";

            var startParam = command.CreateParameter();
            startParam.ParameterName = "@start";
            startParam.Value = start;
            command.Parameters.Add(startParam);

            var endParam = command.CreateParameter();
            endParam.ParameterName = "@end";
            endParam.Value = end;
            command.Parameters.Add(endParam);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var row = new DailyCashupTopupAggRow
                {
                    SystemUserId = reader.GetInt64(reader.GetOrdinal("SystemUserId")),
                    ShiftId = reader.GetInt32(reader.GetOrdinal("ShiftId")),
                    ShiftDate = reader.GetDateTime(reader.GetOrdinal("ShiftDate")),
                    ActualAmount = reader.GetDecimal(reader.GetOrdinal("ActualAmount"))
                };

                if (shiftIds != null && shiftIds.Any() && !shiftIds.Contains(row.ShiftId))
                    continue;

                if (systemUserIds != null && systemUserIds.Any() && !systemUserIds.Contains(row.SystemUserId))
                    continue;

                results.Add(row);
            }

            return results;
        }

        public async Task<DailyCashupResultDto> GetDailyCashupAsync(
            DateTime startDate,
            DateTime endDate,
            List<int>? shiftIds = null,
            List<long>? systemUserIds = null,
            int page = 1,
            int pageSize = 10)
        {
            var start = startDate.Date;
            var end = endDate.Date.AddDays(1).AddTicks(-1);

            shiftIds ??= new List<int>();
            systemUserIds ??= new List<long>();

            var hasShiftFilter = shiftIds.Any();
            var hasOperatorFilter = systemUserIds.Any();

            // =========================================================
            // 1) TRANSACTIONS BASE (Lane Cash only, cash transactions)
            // =========================================================
            var txBase =
                from t in _context.Transactions.AsNoTracking()
                where t.TransactionDateTime >= start
                      && t.TransactionDateTime <= end
                      && t.SystemUserId != null
                      && t.TransactionTypeId == CashTransactionTypeId
                      && (!hasShiftFilter || shiftIds.Contains((int)t.ShiftId))
                      && (!hasOperatorFilter || systemUserIds.Contains(t.SystemUserId.Value))
                select new
                {
                    SystemUserId = t.SystemUserId.Value,
                    ShiftId = (int)t.ShiftId,
                    ShiftDate = t.ShiftDate.Date,
                    NettAmount = (decimal)t.NettAmount
                };

            // =========================================================
            // 2) LOOKUP JOINS FOR TRANSACTIONS
            // =========================================================
            var txWithNames =
                from t in txBase
                join su in _context.SystemUsers.AsNoTracking()
                    on t.SystemUserId equals (long)su.SystemUserId
                join sh in _context.Shifts.AsNoTracking()
                    on t.ShiftId equals (int)sh.ShiftId into shGroup
                from sh in shGroup.DefaultIfEmpty()
                select new
                {
                    t.SystemUserId,
                    t.ShiftId,
                    t.ShiftDate,
                    ShiftDescription = sh != null ? sh.Description : null,
                    TollOperator = su.Username,
                    t.NettAmount
                };

            // =========================================================
            // 3) AGGREGATE TRANSACTIONS TO DAILY GRAIN
            // =========================================================
            var txAgg = await (
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
                    NettAmount = g.Sum(v => v.NettAmount)
                }
            ).ToListAsync();

            // =========================================================
            // 4) AGGREGATE TOP-UPS TO DAILY GRAIN (raw SQL, cash-only)
            // =========================================================
            var topupAgg = await GetCashTopupAggregatesAsync(start, end, shiftIds, systemUserIds);

            // =========================================================
            // 5) CASHUP AGGREGATE (Cash Declared)
            // =========================================================
            var cashupAgg = await (
                from cc in _context.CollectorCashups.AsNoTracking()
                where cc.ShiftDate >= start.Date
                      && cc.ShiftDate <= end.Date
                      && (!hasShiftFilter || shiftIds.Contains((int)cc.ShiftId))
                      && (!hasOperatorFilter || systemUserIds.Contains((long)cc.SystemUserId))
                group cc by new
                {
                    SystemUserId = (long)cc.SystemUserId,
                    ShiftId = (int)cc.ShiftId,
                    ShiftDate = cc.ShiftDate.Date
                }
                into g
                select new
                {
                    g.Key.SystemUserId,
                    g.Key.ShiftId,
                    g.Key.ShiftDate,
                    TotalDeclared = g.Sum(x => (decimal?)x.TotalDeclared) ?? 0m
                }
            ).ToListAsync();

            // =========================================================
            // 6) LOOKUPS FOR FINAL DISPLAY
            // =========================================================
            var users = await _context.SystemUsers.AsNoTracking()
                .Select(u => new
                {
                    SystemUserId = (long)u.SystemUserId,
                    Username = u.Username
                })
                .ToListAsync();

            var shifts = await _context.Shifts.AsNoTracking()
                .Select(s => new
                {
                    ShiftId = (int)s.ShiftId,
                    Description = s.Description
                })
                .ToListAsync();

            // =========================================================
            // 7) BASE KEYS
            // =========================================================
            var baseKeys = txAgg
                .Select(x => new DailyCashupBaseKey
                {
                    SystemUserId = x.SystemUserId,
                    ShiftId = x.ShiftId,
                    ShiftDate = x.ShiftDate
                })
                .Concat(
                    topupAgg.Select(x => new DailyCashupBaseKey
                    {
                        SystemUserId = x.SystemUserId,
                        ShiftId = x.ShiftId,
                        ShiftDate = x.ShiftDate
                    })
                )
                .GroupBy(x => new { x.SystemUserId, x.ShiftId, x.ShiftDate })
                .Select(g => g.First())
                .ToList();

            // =========================================================
            // 8) FINAL ROWS
            // =========================================================
            var fullItems = baseKeys
                .Select(k =>
                {
                    var tx = txAgg.FirstOrDefault(x =>
                        x.SystemUserId == k.SystemUserId &&
                        x.ShiftId == k.ShiftId &&
                        x.ShiftDate == k.ShiftDate);

                    var topUp = topupAgg.FirstOrDefault(x =>
                        x.SystemUserId == k.SystemUserId &&
                        x.ShiftId == k.ShiftId &&
                        x.ShiftDate == k.ShiftDate);

                    var declared = cashupAgg.FirstOrDefault(c =>
                        c.SystemUserId == k.SystemUserId &&
                        c.ShiftId == k.ShiftId &&
                        c.ShiftDate == k.ShiftDate);

                    var user = users.FirstOrDefault(u => u.SystemUserId == k.SystemUserId);
                    var shift = shifts.FirstOrDefault(s => s.ShiftId == k.ShiftId);

                    var laneCash = tx?.NettAmount ?? 0m;
                    var topUps = topUp?.ActualAmount ?? 0m;
                    var cashDeclared = declared?.TotalDeclared ?? 0m;
                    var expected = laneCash + topUps;

                    return new DailyCashupDto
                    {
                        ShiftDate = k.ShiftDate,                                 // Operational Day
                        ShiftDescription = shift?.Description ?? "-- None --",   // Operational Shift
                        TollOperator = user?.Username ?? "-- None --",           // Toll Operator ID

                        NettAmount = laneCash,            // Lane Cash
                        ActualAmount = topUps,            // Top-ups
                        TotalCashExpected = expected,     // Lane Cash + Top-ups
                        TotalDeclared = cashDeclared,     // Cash Declared
                        Difference = cashDeclared - expected, // Surplus /- Shortage
                        TotalBanked = cashDeclared        // Total Banked
                    };
                })
                .OrderBy(x => x.ShiftDate)
                .ThenBy(x => x.ShiftDescription)
                .ThenBy(x => x.TollOperator)
                .ToList();

            // =========================================================
            // 9) PAGINATION
            // =========================================================
            var totalCount = fullItems.Count;
            var totalPages = totalCount == 0
                ? 0
                : (int)Math.Ceiling((double)totalCount / pageSize);

            var items = fullItems
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // =========================================================
            // 10) SHIFT TOTALS
            // =========================================================
            var shiftTotals = fullItems
                .GroupBy(x => x.ShiftDescription)
                .Select(g => new DailyCashupShiftTotalDto
                {
                    ShiftDescription = g.Key,
                    NettAmount = g.Sum(x => x.NettAmount),
                    ActualAmount = g.Sum(x => x.ActualAmount),
                    TotalCashExpected = g.Sum(x => x.TotalCashExpected),
                    TotalDeclared = g.Sum(x => x.TotalDeclared),
                    Difference = g.Sum(x => x.Difference),
                    TotalBanked = g.Sum(x => x.TotalBanked)
                })
                .OrderBy(x => x.ShiftDescription)
                .ToList();

            // =========================================================
            // 11) GRAND TOTAL
            // =========================================================
            var grandTotal = new DailyCashupGrandTotalDto
            {
                NettAmount = fullItems.Sum(x => x.NettAmount),
                ActualAmount = fullItems.Sum(x => x.ActualAmount),
                TotalCashExpected = fullItems.Sum(x => x.TotalCashExpected),
                TotalDeclared = fullItems.Sum(x => x.TotalDeclared),
                Difference = fullItems.Sum(x => x.Difference),
                TotalBanked = fullItems.Sum(x => x.TotalBanked)
            };

            return new DailyCashupResultDto
            {
                FullItems = fullItems,
                Items = items,
                ShiftTotals = shiftTotals,
                GrandTotal = grandTotal,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        public async Task<DailyCashupFilterOptionsDto> GetDailyCashupFilterOptionsAsync()
        {
            var shifts = await _context.Shifts
                .AsNoTracking()
                .Select(s => new FilterItemDto<int>
                {
                    Id = (int)s.ShiftId,
                    Name = s.Description ?? ""
                })
                .Where(x => x.Name != "")
                .OrderBy(x => x.Name)
                .ToListAsync();

            var operators = await _context.SystemUsers
                .AsNoTracking()
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