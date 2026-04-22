using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.DTOs;
using Toll.Reporting.Api.Repositories.Interfaces;
using TollReportingSystem.Data;

namespace Toll.Reporting.Api.Repositories
{
    public class AccountUsageDetailsRepository : IAccountUsageDetailsRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountUsageDetailsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AccountSearchResultDto>> SearchAccountsAsync(string q, int take = 20)
        {
            q = (q ?? string.Empty).Trim();

            if (q.Length < 2)
                return new List<AccountSearchResultDto>();

            take = Math.Clamp(take, 1, 50);

            var results = await _context.RegisteredUsers
                .AsNoTracking()
                .Where(ru =>
                    ru.RegisterUserId.ToString().Contains(q) ||
                    (ru.AccNr != null && ru.AccNr.Contains(q)) ||
                    (ru.CompanyName != null && ru.CompanyName.Contains(q)) ||
                    (ru.FirstName != null && ru.FirstName.Contains(q)) ||
                    (ru.LastName != null && ru.LastName.Contains(q)) ||
                    (((ru.FirstName ?? "") + " " + (ru.LastName ?? "")).Trim()).Contains(q))
                .OrderBy(ru => ru.RegisterUserId)
                .Select(ru => new AccountSearchResultDto
                {
                    AccountNumber = ru.RegisterUserId.ToString(),

                    Description =
                (!string.IsNullOrWhiteSpace(ru.AccNr) ? ru.AccNr : ru.RegisterUserId.ToString()) +
                " - " +
                (!string.IsNullOrWhiteSpace(ru.CompanyName)
                    ? ru.CompanyName
                    : ((ru.FirstName ?? "") + " " + (ru.LastName ?? "")).Trim())
                })
                .Take(take)
                .ToListAsync();

            return results;
        }

        public async Task<AccountUsageDetailsResponseDto> GetAccountUsageDetailsAsync(
            string accountNumber,
            DateTime startDate,
            DateTime endDate)
        {
            var response = new AccountUsageDetailsResponseDto();

            accountNumber = (accountNumber ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(accountNumber))
                return response;

            long accId;

            // Accept either numeric RegisterUserId or AccNr
            if (long.TryParse(accountNumber, out var parsedId))
            {
                accId = parsedId;
            }
            else
            {
                var foundId = await _context.RegisteredUsers
                    .AsNoTracking()
                    .Where(r => r.AccNr == accountNumber)
                    .Select(r => (long?)r.RegisterUserId)
                    .FirstOrDefaultAsync();

                if (!foundId.HasValue)
                    return response;

                accId = foundId.Value;
            }

            var endExclusive =
                endDate.TimeOfDay == TimeSpan.Zero
                    ? endDate.Date.AddDays(1)
                    : endDate.AddMilliseconds(1);

            // =========================================================
            // 1) USER / ACCOUNT HEADER
            // =========================================================
            var user = await _context.RegisteredUsers
                .AsNoTracking()
                .Where(ru => ru.RegisterUserId == accId)
                .Select(ru => new
                {
                    ru.RegisterUserId,
                    ru.AccNr,
                    ru.IsActive,
                    Balance = (decimal?)ru.Balance
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return response;

            var accountStatus =
                user.IsActive == true ? "Active" :
                user.IsActive == false ? "Inactive" :
                "Dormant";

            var currentBalance = user.Balance ?? 0m;

            // =========================================================
            // 2) TOP-UP TOTALS
            // =========================================================
            var totalTopUps = await _context.RegisteredUserTopUps
                .AsNoTracking()
                .Where(rtu =>
                    rtu.RegisterUserId == accId &&
                    rtu.RechargedOn >= startDate &&
                    rtu.RechargedOn < endExclusive)
                .SumAsync(rtu => (decimal?)rtu.Amount) ?? 0m;

            // =========================================================
            // 3) TRANSACTION TOTALS
            // =========================================================
            var totalTransactions = await _context.Transactions
                .AsNoTracking()
                .Where(t =>
                    t.RegisteredUserId == accId &&
                    t.TransactionDateTime >= startDate &&
                    t.TransactionDateTime < endExclusive)
                .SumAsync(t => (decimal?)t.NettAmount) ?? 0m;

            // Opening balance inferred from current balance and in-period movement.
            // This matches the approach we tested before moving back to repository code.
            var openingBalance = currentBalance - totalTopUps + totalTransactions;

            response.Header = new AccountUsageDetailsHeaderDto
            {
                AccountNumber = !string.IsNullOrWhiteSpace(user.AccNr)
                    ? user.AccNr
                    : user.RegisterUserId.ToString(),

                AccountStatus = accountStatus,
                OpeningBalance = openingBalance,
                TotalTopUps = totalTopUps,
                TotalTransactions = totalTransactions,
                TotalFees = 0m,
                TotalDeposits = 0m,
                TotalRefunds = 0m,
                ClosingBalance = currentBalance,
                DepositRefunded = 0m,
                StartDate = startDate,
                EndDate = endDate
            };

            // =========================================================
            // 4) LATEST ACTIVE IDENTIFIERS
            // =========================================================
            var identifiers = await _context.RegisteredUserIdentifiers
                .AsNoTracking()
                .Where(id => id.RegisteredUserId == accId && id.IsActive == true)
                .Select(id => new
                {
                    id.RegisteredUserId,
                    id.RegisteredIdentifier,
                    id.NumberPlateDetails,
                    id.ActivationDate,
                    id.RegisteredUserIdentifierId
                })
                .ToListAsync();

            var latestIdentifiers = identifiers
                .GroupBy(x => new
                {
                    x.RegisteredUserId,
                    RegisteredIdentifier = x.RegisteredIdentifier ?? string.Empty
                })
                .Select(g => g
                    .OrderByDescending(x => x.ActivationDate)
                    .ThenByDescending(x => x.RegisteredUserIdentifierId)
                    .First())
                .ToList();

            // =========================================================
            // 5) TRANSACTION GROUPS BY IDENTIFIER
            // =========================================================
            var transactionGroups = await _context.Transactions
                .AsNoTracking()
                .Where(t =>
                    t.RegisteredUserId == accId &&
                    t.TransactionDateTime >= startDate &&
                    t.TransactionDateTime < endExclusive)
                .GroupBy(t => new
                {
                    t.RegisteredUserId,
                    RegisteredIdentifier = t.RegisteredIdentifier ?? string.Empty
                })
                .Select(g => new
                {
                    g.Key.RegisteredUserId,
                    g.Key.RegisteredIdentifier,
                    LaneTransactionCount = g.Count(),
                    LaneTransactionValue = g.Sum(x => (decimal?)x.NettAmount) ?? 0m
                })
                .ToListAsync();

            // =========================================================
            // 6) DETAILS
            // =========================================================
            var details = new List<AccountUsageDetailsItemDto>();

            if (latestIdentifiers.Any())
            {
                foreach (var li in latestIdentifiers)
                {
                    var registeredIdentifier = li.RegisteredIdentifier ?? string.Empty;

                    var trx = transactionGroups.FirstOrDefault(x =>
                        x.RegisteredUserId == li.RegisteredUserId &&
                        x.RegisteredIdentifier == registeredIdentifier);

                    details.Add(new AccountUsageDetailsItemDto
                    {
                        EID_DeviceType = "E-ID",
                        EID_DeviceNumber = li.RegisteredIdentifier ?? "N/A",
                        VehicleRegNumber = li.NumberPlateDetails ?? "N/A",
                        VehicleClass = "N/A",
                        Balance = currentBalance,

                        LaneTransactionCount = trx?.LaneTransactionCount ?? 0,
                        LaneTransactionValue = trx?.LaneTransactionValue ?? 0m,

                        // Put top-up on the first row only to avoid duplication
                        ReceiptTopUp = details.Count == 0 ? totalTopUps : 0m,
                        ReceiptDeposit = 0m,

                        PaymentFees = 0m,
                        PaymentRefunds = 0m,

                        RefundAccount = 0m,
                        RefundDeposit = 0m
                    });
                }
            }
            else
            {
                details.Add(new AccountUsageDetailsItemDto
                {
                    EID_DeviceType = "N/A",
                    EID_DeviceNumber = "N/A",
                    VehicleRegNumber = "N/A",
                    VehicleClass = "N/A",
                    Balance = currentBalance,

                    LaneTransactionCount = transactionGroups.Sum(x => x.LaneTransactionCount),
                    LaneTransactionValue = transactionGroups.Sum(x => x.LaneTransactionValue),

                    ReceiptTopUp = totalTopUps,
                    ReceiptDeposit = 0m,

                    PaymentFees = 0m,
                    PaymentRefunds = 0m,

                    RefundAccount = 0m,
                    RefundDeposit = 0m
                });
            }

            response.Details = details;
            return response;
        }
    }
}