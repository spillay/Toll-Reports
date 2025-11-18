using System;
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

        public async Task<PagedResult<TopUpDto>> GetTopUpsAsync(
            DateTime startDate,
            DateTime endDate,
            string? shift = null,
            string? operatorId = null,
            string? lane = null,
            string? accountNumber = null,
            int page = 1,
            int pageSize = 30)
        {
            // Ensure endDate covers entire end day
            if (endDate.TimeOfDay == TimeSpan.Zero)
                endDate = endDate.AddDays(1).AddSeconds(-1);

            // -------------------------
            // BASE QUERY (strongly typed)
            // -------------------------
            var query =
                from rut in _context.RegisteredUserTopUps.AsNoTracking()
                join ru in _context.RegisteredUsers.AsNoTracking()
                    on rut.RegisterUserId equals ru.RegisterUserId into ruGroup
                from ru in ruGroup.DefaultIfEmpty()
                join pm in _context.PaymentMethods.AsNoTracking()
                    on rut.PaymentMethodId equals pm.PaymentMethodId into pmGroup
                from pm in pmGroup.DefaultIfEmpty()
                where rut.RechargedOn >= startDate && rut.RechargedOn <= endDate
                select new
                {
                    TopUp = rut,
                    User = ru,
                    Payment = pm
                };

            // -------------------------
            // APPLY FILTERS (still in SQL)
            // -------------------------

            if (!string.IsNullOrWhiteSpace(shift))
            {
                query = query.Where(x =>
                    x.TopUp.RechargeShift.ToString() == shift);
            }

            if (!string.IsNullOrWhiteSpace(operatorId))
            {
                query = query.Where(x =>
                    x.TopUp.SystemUserId.ToString() == operatorId);
            }

            if (!string.IsNullOrWhiteSpace(lane))
            {
                query = query.Where(x =>
                    x.TopUp.RechargeStation == lane);
            }

            if (!string.IsNullOrWhiteSpace(accountNumber))
            {
                var acc = accountNumber;

                // Try to treat it also as numeric RegisterUserId
                bool isNumeric = long.TryParse(acc, out var accId);

                query = query.Where(x =>
                    // Match on identifier (RegisteredUserIdentifier)
                    (x.User != null && x.User.RegisteredUserIdentifiers
                        .Any(i => i.RegisteredIdentifier == acc))
                    ||
                    // OR fallback to RegisterUserId if numeric
                    (isNumeric && x.User != null && x.User.RegisterUserId == accId)
                );
            }

            // -------------------------
            // COUNT BEFORE PAGING
            // -------------------------
            var totalCount = await query.CountAsync();

            // -------------------------
            // PAGED RESULT (SQL LEVEL)
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
                    Operator = x.TopUp.SystemUserId.ToString(),

                    AccountNumber = x.User != null &&
                                    x.User.RegisteredUserIdentifiers.Any()
                        ? x.User.RegisteredUserIdentifiers
                            .OrderBy(i => i.RegisteredIdentifier) 
                            .Select(i => i.RegisteredIdentifier)
                            .FirstOrDefault()
                        : (x.User != null
                            ? x.User.RegisterUserId.ToString()
                            : string.Empty),

                    AccountName = x.User != null
                        ? (x.User.CompanyName ?? string.Empty)
                        : string.Empty,

                    AmountPaid = x.TopUp.Amount,
                    MethodOfPayment = x.Payment != null
                        ? (x.Payment.Description ?? string.Empty)
                        : string.Empty
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
    }
}
