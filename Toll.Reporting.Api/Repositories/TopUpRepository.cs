using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.DTOs;
using Toll.Reporting.Api.Repositories.Interfaces;
using TollReportingSystem.Data;

namespace Toll.Reporting.Api.Repositories
{
    public class TopUpRepository : ITopUpRepository
    {
        private readonly ApplicationDbContext _context;

        public TopUpRepository(ApplicationDbContext context)
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

        public async Task<PagedResult<TopUpDto>> GetTopUpsAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? shifts = null,
            List<string>? operatorIds = null,
            List<string>? lanes = null,
            List<string>? paymentMethods = null,
            string? accountNumber = null,
            int page = 1,
            int pageSize = 30)
        {
            if (endDate.TimeOfDay == TimeSpan.Zero)
                endDate = endDate.AddDays(1).AddSeconds(-1);

            shifts = NormalizeList(shifts);
            operatorIds = NormalizeList(operatorIds);
            lanes = NormalizeList(lanes);
            paymentMethods = NormalizeList(paymentMethods);

            // -------------------------
            // BASE QUERY
            // -------------------------
            var query =
                from rut in _context.RegisteredUserTopUps.AsNoTracking()

                join ru in _context.RegisteredUsers.AsNoTracking()
                    on rut.RegisterUserId equals ru.RegisterUserId into ruGroup
                from ru in ruGroup.DefaultIfEmpty()

                join pm in _context.PaymentMethods.AsNoTracking()
                    on rut.PaymentMethodId equals pm.PaymentMethodId into pmGroup
                from pm in pmGroup.DefaultIfEmpty()

                join su in _context.SystemUsers.AsNoTracking()
                    on rut.SystemUserId equals su.SystemUserId into suGroup
                from su in suGroup.DefaultIfEmpty()

                where rut.RechargedOn >= startDate && rut.RechargedOn <= endDate

                select new
                {
                    TopUp = rut,
                    User = ru,
                    Payment = pm,
                    SystemUser = su
                };

            if (shifts != null)
            {
                query = query.Where(x => shifts.Contains(x.TopUp.RechargeShift.ToString()));
            }

            if (operatorIds != null)
            {
                // Filtering by unique ID (matches filter-options returning SystemUserId strings)
                query = query.Where(x => operatorIds.Contains(x.TopUp.SystemUserId.ToString()));
            }

            if (lanes != null)
            {
                query = query.Where(x => lanes.Contains(x.TopUp.RechargeStation));
            }

            if (paymentMethods != null)
            {
                // Trim DB value to avoid trailing-space mismatches
                query = query.Where(x =>
                    x.Payment != null &&
                    x.Payment.Description != null &&
                    paymentMethods.Contains(x.Payment.Description.Trim()));
            }

            if (!string.IsNullOrWhiteSpace(accountNumber))
            {
                var acc = accountNumber.Trim();
                bool isNumeric = long.TryParse(acc, out var accId);

                query = query.Where(x =>
                    (x.User != null && x.User.RegisteredUserIdentifiers.Any(i => i.RegisteredIdentifier == acc))
                    || (isNumeric && x.User != null && x.User.RegisterUserId == accId)
                );
            }

            // -------------------------
            // COUNT BEFORE PAGING
            // -------------------------
            var totalCount = await query.CountAsync();

            // -------------------------
            // PAGED RESULT
            // -------------------------
            var items = await query
                .OrderByDescending(x => x.TopUp.RechargedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new TopUpDto
                {
                    TopUpNumber = (int)x.TopUp.RegisteredUserTopUpId,
                    TopUpDateTime = x.TopUp.RechargedOn,
                    LaneWorkstation = x.TopUp.RechargeStation ?? string.Empty,
                    Shift = x.TopUp.RechargeShift.ToString(),

                    Operator = x.SystemUser != null && !string.IsNullOrWhiteSpace(x.SystemUser.Username)
                        ? x.SystemUser.Username
                        : x.TopUp.SystemUserId.ToString(),

                    AccountNumber = x.User != null && x.User.RegisteredUserIdentifiers.Any()
                        ? x.User.RegisteredUserIdentifiers
                            .OrderBy(i => i.RegisteredIdentifier)
                            .Select(i => i.RegisteredIdentifier)
                            .FirstOrDefault()
                        : (x.User != null ? x.User.RegisterUserId.ToString() : string.Empty),

                    AccountName = x.User != null ? (x.User.CompanyName ?? string.Empty) : string.Empty,
                    AmountPaid = (decimal)x.TopUp.Amount,
                    MethodOfPayment = x.Payment != null ? (x.Payment.Description ?? string.Empty) : string.Empty
                })
                .ToListAsync();

            return new PagedResult<TopUpDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<TopUpDto> GetTopUpFilterOptionsAsync()
        {
            var shifts = await _context.Shifts
                .AsNoTracking()
                .Select(s => s.Description)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            var operators = await _context.SystemUsers
                .AsNoTracking()
                .Where(u => u.SystemUserId > 0)
                .Select(u => u.SystemUserId.ToString())
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            var lanes = await _context.Lanes
                .AsNoTracking()
                .Select(l => l.LaneName)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            var payments = await _context.PaymentMethods
                .AsNoTracking()
                .Select(p => p.Description)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            return new TopUpDto
            {
                ShiftOptions = shifts,
                OperatorOptions = operators,
                LaneOptions = lanes,
                PaymentMethodOptions = payments
            };
        }
    }
}