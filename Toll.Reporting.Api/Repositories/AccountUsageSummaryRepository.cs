using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.DTOs;
using Toll.Reporting.Api.Repositories.Interfaces;
using TollReportingSystem.Data;

namespace Toll.Reporting.Api.Repositories
{
    public class AccountUsageSummaryRepository : IAccountUsageSummaryRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountUsageSummaryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AccountUsageSummaryReportDto> GetSummaryAsync(
            DateTime startDate,
            DateTime endDate,
            string? accountNumber = null,
            int page = 1,
            int pageSize = 20)
        {
            var response = new AccountUsageSummaryReportDto();

            accountNumber = (accountNumber ?? string.Empty).Trim();

            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 20;

            if (pageSize > 200)
                pageSize = 200;

            var endExclusive =
                endDate.TimeOfDay == TimeSpan.Zero
                    ? endDate.Date.AddDays(1)
                    : endDate.AddMilliseconds(1);

            // =========================================================
            // 1) BASE ACCOUNTS
            // =========================================================
            var baseAccountsQuery = _context.RegisteredUsers
                .AsNoTracking()
                .Select(ru => new
                {
                    ru.RegisterUserId,
                    AccountNumber = !string.IsNullOrWhiteSpace(ru.AccNr)
                        ? ru.AccNr
                        : ru.RegisterUserId.ToString(),
                    AccountStatus =
                        ru.IsActive == true ? "Active" :
                        ru.IsActive == false ? "Inactive" :
                        "Dormant",
                    ClosingBalance = (decimal?)ru.Balance ?? 0m
                });

            if (!string.IsNullOrWhiteSpace(accountNumber))
            {
                baseAccountsQuery = baseAccountsQuery.Where(x =>
                    x.AccountNumber == accountNumber ||
                    x.RegisterUserId.ToString() == accountNumber);
            }

            var baseAccounts = await baseAccountsQuery.ToListAsync();

            // =========================================================
            // 2) TOP-UPS PER ACCOUNT
            // =========================================================
            var topUps = await _context.RegisteredUserTopUps
                .AsNoTracking()
                .Where(rtu =>
                    rtu.RechargedOn >= startDate &&
                    rtu.RechargedOn < endExclusive)
                .GroupBy(rtu => rtu.RegisterUserId)
                .Select(g => new
                {
                    RegisterUserId = g.Key,
                    TopUps = g.Sum(x => (decimal?)x.Amount) ?? 0m
                })
                .ToListAsync();

            var topUpsMap = topUps.ToDictionary(
                x => x.RegisterUserId,
                x => x.TopUps
            );

            // =========================================================
            // 3) TRANSACTIONS PER ACCOUNT
            // =========================================================
            var transactionAgg = await _context.Transactions
                .AsNoTracking()
                .Where(t =>
                    t.RegisteredUserId != null &&
                    t.TransactionDateTime >= startDate &&
                    t.TransactionDateTime < endExclusive)
                .GroupBy(t => t.RegisteredUserId)
                .Select(g => new
                {
                    RegisteredUserId = g.Key,
                    LaneTransactionCount = g.Count(),
                    LaneTransactionValue = g.Sum(x => (decimal?)x.NettAmount) ?? 0m
                })
                .ToListAsync();

            var transactionMap = transactionAgg
                .Where(x => x.RegisteredUserId != null)
                .ToDictionary(
                    x => x.RegisteredUserId!.Value,
                    x => new
                    {
                        x.LaneTransactionCount,
                        x.LaneTransactionValue
                    });

            // =========================================================
            // 4) BUILD ITEMS IN MEMORY
            // discounts remain disabled for now
            // =========================================================
            var allItems = new List<AccountUsageSummaryItemDto>();

            foreach (var ba in baseAccounts)
            {
                var topUpValue = topUpsMap.TryGetValue(ba.RegisterUserId, out var tu)
                    ? tu
                    : 0m;

                var hasTransaction = transactionMap.TryGetValue(ba.RegisterUserId, out var trx);

                var laneTransactionCount = hasTransaction ? trx!.LaneTransactionCount : 0;
                var laneTransactionValue = hasTransaction ? trx!.LaneTransactionValue : 0m;

                allItems.Add(new AccountUsageSummaryItemDto
                {
                    AccountNumber = ba.AccountNumber,
                    AccountStatus = ba.AccountStatus,

                    OpeningBalance = ba.ClosingBalance - topUpValue + laneTransactionValue,
                    ClosingBalance = ba.ClosingBalance,

                    LaneTransactionCount = laneTransactionCount,
                    LaneTransactionValue = laneTransactionValue,

                    // discounts off for now
                    LaneDiscountCount = 0,
                    LaneDiscountValue = 0m,

                    ReceiptTopUp = topUpValue,
                    ReceiptDeposit = 0m,

                    PaymentFees = 0m,
                    PaymentRefunds = 0m,

                    RefundAccount = 0m,
                    RefundDeposit = 0m
                });
            }

            allItems = allItems
                .OrderBy(x =>
                {
                    var isNumeric = long.TryParse(x.AccountNumber, out _);
                    return isNumeric ? 0 : 1;
                })
                .ThenBy(x =>
                {
                    var ok = long.TryParse(x.AccountNumber, out var number);
                    return ok ? number : long.MaxValue;
                })
                .ThenBy(x => x.AccountNumber)
                .ToList();

            // =========================================================
            // 5) PAGINATION
            // =========================================================
            var totalCount = allItems.Count;
            var totalPages = totalCount == 0
                ? 0
                : (int)Math.Ceiling(totalCount / (double)pageSize);

            var pagedItems = allItems
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // =========================================================
            // 6) TOTALS / HEADER
            // =========================================================
            var totals = new AccountUsageSummaryTotalsDto
            {
                TotalAccounts = await _context.RegisteredUsers
                    .AsNoTracking()
                    .CountAsync(),

                Active = await _context.RegisteredUsers
                    .AsNoTracking()
                    .CountAsync(x => x.IsActive == true),

                Terminated = await _context.RegisteredUsers
                    .AsNoTracking()
                    .CountAsync(x => x.IsActive == false),

                Dormant = await _context.RegisteredUsers
                    .AsNoTracking()
                    .CountAsync(x => x.IsActive == null),

                TotalEIdDevices = await _context.RegisteredUserIdentifiers
                    .AsNoTracking()
                    .CountAsync(),

                TotalEtcTags = await _context.RegisteredUserIdentifiers
                    .AsNoTracking()
                    .CountAsync(x => x.IdentifierType.Description == "ETC"),

                TotalSmartCards = await _context.RegisteredUserIdentifiers
                    .AsNoTracking()
                    .CountAsync(x => x.IdentifierType.Description == "SmartCard"),

                StartDate = startDate,
                EndDate = endDate
            };

            response.Summary = totals;
            response.Data = new global::PagedResult<AccountUsageSummaryItemDto>
            {
                FullItems = new List<AccountUsageSummaryItemDto>(),
                Items = pagedItems,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return response;
        }
    }
}
