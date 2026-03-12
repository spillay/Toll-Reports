using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.DTOs;
using TollReportingSystem.Data;

namespace Toll.Reporting.Api.Repositories.Implementations;

public class AccountHistoryRepository : IAccountHistoryRepository
{
    private readonly ApplicationDbContext _context;

    public AccountHistoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AccountHistoryDto> GetAccountHistoryAsync(
        string accountNumber,
        DateTime? startDate,
        DateTime? endDate)
    {
        var start = startDate ?? DateTime.MinValue;
        var end = endDate ?? DateTime.MaxValue;

        var endExclusive =
            (endDate.HasValue && endDate.Value.TimeOfDay == TimeSpan.Zero)
                ? endDate.Value.Date.AddDays(1)
                : end.AddMilliseconds(1);

        const string DefaultLaneName = "Lekki-Ikoyi";
        const string DefaultPaymentMethod = "SmartCard";

        // -----------------------------
        // ALL ACCOUNTS
        // -----------------------------
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            var topUps = await BuildTopUpsQuery(start, endExclusive, accId: null)
                .OrderByDescending(x => x.RechargedOn)
                .Take(5000)
                .ToListAsync();

            var trxs = await BuildTransactionsQuery(start, endExclusive, accId: null, DefaultLaneName)
                .OrderByDescending(x => x.TransactionDateTime)
                .Take(5000)
                .ToListAsync();

            var all = topUps.Select(x => new AccountHistoryRecordDto
            {
                LaneName = DefaultLaneName,
                TransactionType = "Top-up",
                TransactionAmount = 0m,
                TopUpAmount = x.Amount,
                UserBalance = x.Balance,
                PaymentMethod = x.PaymentMethod,
                TransactionDateTime = x.RechargedOn,
                RegisteredIdentifier = x.RegisteredIdentifier ?? "N/A",
                NumberPlate = x.NumberPlate ?? "N/A",
                Description = $"Account {x.RegisterUserId} ({(x.FirstName ?? "")} {(x.LastName ?? "")})".Trim()
            })
                .Concat(trxs.Select(x => new AccountHistoryRecordDto
                {
                    LaneName = x.LaneName,
                    TransactionType = x.TrxType,
                    TransactionAmount = x.NettAmount,
                    TopUpAmount = 0m,
                    UserBalance = x.Balance,
                    PaymentMethod = DefaultPaymentMethod,
                    TransactionDateTime = x.TransactionDateTime,
                    RegisteredIdentifier = x.RegisteredIdentifier ?? "N/A",
                    NumberPlate = x.NumberPlate ?? "N/A",
                    Description = x.AccountId.HasValue
                        ? $"Account {x.AccountId.Value} ({(x.FirstName ?? "")} {(x.LastName ?? "")})".Trim()
                        : "Unregistered Transaction"
                }))
                .OrderByDescending(x => x.TransactionDateTime)
                .Take(5000)
                .ToList();

            return new AccountHistoryDto { AccountHeader = null, HistoryRecords = all };
        }

        // -----------------------------
        // SPECIFIC ACCOUNT
        // -----------------------------
        if (!long.TryParse(accountNumber.Trim(), out var accId))
            return new AccountHistoryDto { AccountHeader = null, HistoryRecords = new List<AccountHistoryRecordDto>() };

        var accountHeader = await _context.RegisteredUsers.AsNoTracking()
            .Where(ru => ru.RegisterUserId == accId)
            .Select(ru => new AccountHeaderDto
            {
                AccountNumber = ru.RegisterUserId.ToString(),
                AccountHolder = ru.CompanyName ?? ((ru.FirstName ?? "") + " " + (ru.LastName ?? "")).Trim(),
                AccountStatus = (ru.IsActive ?? false) ? "Active" : "Inactive",
                AccountType = (ru.IsPrepaid ?? false) ? "Prepaid" : "Postpaid",
                MobileNumber = ru.PrimaryContact ?? "N/A",
                Email = ru.PrimaryEmail ?? "N/A",
                AccountBalance = (decimal)ru.Balance
            })
            .FirstOrDefaultAsync();

        if (accountHeader == null)
            return new AccountHistoryDto { AccountHeader = null, HistoryRecords = new List<AccountHistoryRecordDto>() };

        var accTopUps = await BuildTopUpsQuery(start, endExclusive, accId)
            .OrderByDescending(x => x.RechargedOn)
            .ToListAsync();

        var accTrxs = await BuildTransactionsQuery(start, endExclusive, accId, DefaultLaneName)
            .OrderByDescending(x => x.TransactionDateTime)
            .ToListAsync();

        var history = accTrxs.Select(x => new AccountHistoryRecordDto
        {
            LaneName = x.LaneName,
            TransactionType = x.TrxType,
            TransactionAmount = x.NettAmount,
            TopUpAmount = 0m,
            UserBalance = x.Balance,
            PaymentMethod = DefaultPaymentMethod,
            TransactionDateTime = x.TransactionDateTime,
            RegisteredIdentifier = x.RegisteredIdentifier ?? "N/A",
            NumberPlate = x.NumberPlate ?? "N/A",
            Description = "Lane Transaction"
        })
            .Concat(accTopUps.Select(x => new AccountHistoryRecordDto
            {
                LaneName = DefaultLaneName,
                TransactionType = "Top-up",
                TransactionAmount = 0m,
                TopUpAmount = x.Amount,
                UserBalance = x.Balance,
                PaymentMethod = string.IsNullOrWhiteSpace(x.PaymentMethod) ? "Back-Office" : x.PaymentMethod,
                TransactionDateTime = x.RechargedOn,
                RegisteredIdentifier = x.RegisteredIdentifier ?? "N/A",
                NumberPlate = x.NumberPlate ?? "N/A",
                Description = "Account Top-up"
            }))
            .OrderByDescending(x => x.TransactionDateTime)
            .Take(3000)
            .ToList();

        return new AccountHistoryDto { AccountHeader = accountHeader, HistoryRecords = history };
    }

    // ------------------------------------------------------------------
    // Query Builders (projection-only; avoids materializing full entities)
    // ------------------------------------------------------------------

    private IQueryable<TopUpRow> BuildTopUpsQuery(DateTime start, DateTime endExclusive, long? accId)
    {
        var q =
            from rtu in _context.RegisteredUserTopUps.AsNoTracking()
            join ru in _context.RegisteredUsers.AsNoTracking()
                on rtu.RegisterUserId equals ru.RegisterUserId
            join pm0 in _context.PaymentMethods.AsNoTracking()
                on rtu.PaymentMethodId equals pm0.PaymentMethodId into pmGroup
            from pm in pmGroup.DefaultIfEmpty()

            let latestId =
                (from id in _context.RegisteredUserIdentifiers.AsNoTracking()
                 where id.IsActive == true && id.RegisteredUserId == ru.RegisterUserId
                 orderby id.ActivationDate descending, id.RegisteredUserIdentifierId descending
                 select new { id.RegisteredIdentifier, id.NumberPlateDetails })
                .FirstOrDefault()

            where rtu.RechargedOn >= start && rtu.RechargedOn < endExclusive
            select new TopUpRow
            {
                RegisterUserId = ru.RegisterUserId,
                FirstName = ru.FirstName,
                LastName = ru.LastName,
                Balance = (decimal)ru.Balance,

                RechargedOn = rtu.RechargedOn,
                Amount = (decimal)rtu.Amount,

                PaymentMethod = pm != null ? pm.Description : "N/A",
                RegisteredIdentifier = latestId != null ? latestId.RegisteredIdentifier : null,
                NumberPlate = latestId != null ? latestId.NumberPlateDetails : null
            };

        if (accId.HasValue)
            q = q.Where(x => x.RegisterUserId == accId.Value);

        return q;
    }

    private IQueryable<TransactionRow> BuildTransactionsQuery(DateTime start, DateTime endExclusive, long? accId, string defaultLaneName)
    {
        var q =
            from t in _context.Transactions.AsNoTracking()
            join ru0 in _context.RegisteredUsers.AsNoTracking()
                on t.RegisteredUserId equals ru0.RegisterUserId into ruGroup
            from ru in ruGroup.DefaultIfEmpty()
            join l0 in _context.Lanes.AsNoTracking()
                on t.LaneId equals l0.LaneId into lGroup
            from l in lGroup.DefaultIfEmpty()
            join tt0 in _context.TransactionTypes.AsNoTracking()
                on t.TransactionTypeId equals tt0.TransactionTypeId into ttGroup
            from tt in ttGroup.DefaultIfEmpty()

            let latestId =
                (from id in _context.RegisteredUserIdentifiers.AsNoTracking()
                 where id.IsActive == true && id.RegisteredUserId == t.RegisteredUserId
                 orderby id.ActivationDate descending, id.RegisteredUserIdentifierId descending
                 select new { id.RegisteredIdentifier, id.NumberPlateDetails })
                .FirstOrDefault()

            where t.TransactionDateTime >= start && t.TransactionDateTime < endExclusive
            select new TransactionRow
            {
                AccountId = ru != null ? (long?)ru.RegisterUserId : null,
                FirstName = ru != null ? ru.FirstName : null,
                LastName = ru != null ? ru.LastName : null,
                Balance = ru != null ? (decimal)ru.Balance : 0m,

                TransactionDateTime = t.TransactionDateTime,
                NettAmount = (decimal)t.NettAmount,

                LaneName = l != null ? l.LaneName : defaultLaneName,
                TrxType = tt != null ? tt.Description : "Lane Transaction",

                RegisteredIdentifier =
                    !string.IsNullOrWhiteSpace(t.RegisteredIdentifier)
                        ? t.RegisteredIdentifier
                        : (latestId != null ? latestId.RegisteredIdentifier : null),

                NumberPlate = latestId != null ? latestId.NumberPlateDetails : null
            };

        if (accId.HasValue)
            q = q.Where(x => x.AccountId == accId.Value);

        return q;
    }

    // ------------------------------------------------------------------
    // Small private rows (simple DTO-like classes)
    // ------------------------------------------------------------------

    private sealed class TopUpRow
    {
        public long RegisterUserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public decimal Balance { get; set; }

        public DateTime RechargedOn { get; set; }
        public decimal Amount { get; set; }

        public string? PaymentMethod { get; set; }
        public string? RegisteredIdentifier { get; set; }
        public string? NumberPlate { get; set; }
    }

    private sealed class TransactionRow
    {
        public long? AccountId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public decimal Balance { get; set; }

        public DateTime TransactionDateTime { get; set; }
        public decimal NettAmount { get; set; }

        public string LaneName { get; set; } = "";
        public string TrxType { get; set; } = "";

        public string? RegisteredIdentifier { get; set; }
        public string? NumberPlate { get; set; }
    }
}