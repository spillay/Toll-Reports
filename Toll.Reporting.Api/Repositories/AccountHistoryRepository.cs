using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.DTOs;
using Toll.Reporting.Api.Repositories.Interfaces;
using TollReportingSystem.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Toll.Reporting.Api.Repositories.Implementations
{
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
            // Normalize dates
            DateTime start = startDate?.ToUniversalTime() ?? DateTime.MinValue;
            DateTime end = endDate?.ToUniversalTime() ?? DateTime.MaxValue;

            if (endDate.HasValue && endDate.Value.TimeOfDay == TimeSpan.Zero)
                end = endDate.Value.Date.AddDays(1).AddSeconds(-1).ToUniversalTime();

            // ==============================================================
            // CASE 1 — ALL ACCOUNTS
            // ==============================================================
            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                // --- TOP UPS (executes safely in SQL)
                var tops = await (
                    from rtu in _context.RegisteredUserTopUps.AsNoTracking()
                    join ru in _context.RegisteredUsers.AsNoTracking()
                        on rtu.RegisterUserId equals ru.RegisterUserId
                    join pm in _context.PaymentMethods.AsNoTracking()
                        on rtu.PaymentMethodId equals pm.PaymentMethodId into pmGroup
                    from pm in pmGroup.DefaultIfEmpty()
                    where rtu.RechargedOn >= start && rtu.RechargedOn <= end
                    select new AccountHistoryRecordDto
                    {
                        LaneName = "Lekki-Ikoyi",
                        TransactionType = "Top-up",
                        TransactionAmount = 0m,
                        TopUpAmount = (decimal)rtu.Amount,
                        UserBalance = (decimal)ru.Balance,
                        PaymentMethod = pm.Description ?? "N/A",
                        TransactionDateTime = rtu.RechargedOn,
                        RegisteredIdentifier = "N/A",
                        NumberPlate = "N/A",
                        Description = $"Account {ru.RegisterUserId} ({ru.FirstName} {ru.LastName})"
                    }
                ).ToListAsync();

                // --- TRANSACTIONS (executes safely in SQL)
                var trxs = await (
                    from t in _context.Transactions.AsNoTracking()
                    join ru in _context.RegisteredUsers.AsNoTracking()
                        on t.RegisteredUserId equals ru.RegisterUserId
                    join l in _context.Lanes.AsNoTracking()
                        on t.LaneId equals l.LaneId into lGroup
                    from l in lGroup.DefaultIfEmpty()
                    join tt in _context.TransactionTypes.AsNoTracking()
                        on t.TransactionTypeId equals tt.TransactionTypeId into ttGroup
                    from tt in ttGroup.DefaultIfEmpty()
                    where t.TransactionDateTime >= start && t.TransactionDateTime <= end
                    select new AccountHistoryRecordDto
                    {
                        LaneName = l.LaneName ?? "Lekki-Ikoyi",
                        TransactionType = tt.Description ?? "Lane Transaction",
                        TransactionAmount = (decimal)t.NettAmount,
                        TopUpAmount = 0m,
                        UserBalance = (decimal)ru.Balance,
                        PaymentMethod = "SmartCard",
                        TransactionDateTime = t.TransactionDateTime,
                        RegisteredIdentifier = "N/A",
                        NumberPlate = "N/A",
                        Description = $"Account {ru.RegisterUserId} ({ru.FirstName} {ru.LastName})"
                    }
                ).ToListAsync();

                // --- MERGE IN MEMORY (solves the EF translation problem)
                var all = tops.Concat(trxs)
                              .OrderByDescending(x => x.TransactionDateTime)
                              .Take(5000)
                              .ToList();

                return new AccountHistoryDto
                {
                    AccountHeader = null,
                    HistoryRecords = all
                };
            }

            // ==================================================================
            // CASE 2 — SPECIFIC ACCOUNT (this part was already working)
            // ==================================================================
            long accId = Convert.ToInt64(accountNumber.Trim());

            var accountHeader = await _context.RegisteredUsers
                .Where(ru => ru.RegisterUserId == accId)
                .Select(ru => new AccountHeaderDto
                {
                    AccountNumber = ru.RegisterUserId.ToString(),
                    AccountHolder = ru.CompanyName ?? $"{ru.FirstName} {ru.LastName}",
                    AccountStatus = ru.IsActive == true ? "Active" : "Inactive",
                    AccountType = ru.IsPrepaid == true ? "Prepaid" : "Postpaid",
                    MobileNumber = ru.PrimaryContact ?? "N/A",
                    Email = ru.PrimaryEmail ?? "N/A",
                    AccountBalance = (decimal)ru.Balance
                })
                .FirstOrDefaultAsync();

            if (accountHeader == null)
                return new AccountHistoryDto { HistoryRecords = new() };

            // TRANSACTIONS
            var accTrx = await (
                from t in _context.Transactions.AsNoTracking()
                join ru in _context.RegisteredUsers.AsNoTracking()
                    on t.RegisteredUserId equals ru.RegisterUserId
                join l in _context.Lanes.AsNoTracking()
                    on t.LaneId equals l.LaneId into lGroup
                from l in lGroup.DefaultIfEmpty()
                join tt in _context.TransactionTypes.AsNoTracking()
                    on t.TransactionTypeId equals tt.TransactionTypeId into ttGroup
                from tt in ttGroup.DefaultIfEmpty()
                where ru.RegisterUserId == accId
                      && t.TransactionDateTime >= start
                      && t.TransactionDateTime <= end
                select new AccountHistoryRecordDto
                {
                    LaneName = l.LaneName ?? "Lekki-Ikoyi",
                    TransactionType = tt.Description ?? "Lane Transaction",
                    TransactionAmount = (decimal)t.NettAmount,
                    TopUpAmount = 0m,
                    UserBalance = (decimal)ru.Balance,
                    PaymentMethod = "SmartCard",
                    TransactionDateTime = t.TransactionDateTime,
                    RegisteredIdentifier = "N/A",
                    NumberPlate = "N/A",
                    Description = "Lane Transaction"
                }
            ).ToListAsync();

            // TOP-UPS
            var accTopUps = await (
                from rtu in _context.RegisteredUserTopUps.AsNoTracking()
                join ru in _context.RegisteredUsers.AsNoTracking()
                    on rtu.RegisterUserId equals ru.RegisterUserId
                join pm in _context.PaymentMethods.AsNoTracking()
                    on rtu.PaymentMethodId equals pm.PaymentMethodId into pmGroup
                from pm in pmGroup.DefaultIfEmpty()
                where ru.RegisterUserId == accId
                      && rtu.RechargedOn >= start
                      && rtu.RechargedOn <= end
                select new AccountHistoryRecordDto
                {
                    LaneName = "Lekki-Ikoyi",
                    TransactionType = "Top-up",
                    TransactionAmount = 0m,
                    TopUpAmount = (decimal)rtu.Amount,
                    UserBalance = (decimal)ru.Balance,
                    PaymentMethod = pm.Description ?? "Back-Office",
                    TransactionDateTime = rtu.RechargedOn,
                    RegisteredIdentifier = "N/A",
                    NumberPlate = "N/A",
                    Description = "Account Top-up"
                }
            ).ToListAsync();

            // FINAL MERGE (safe)
            var history = accTrx.Concat(accTopUps)
                                .OrderBy(x => x.TransactionDateTime)
                                .Take(3000)
                                .ToList();

            return new AccountHistoryDto
            {
                AccountHeader = accountHeader,
                HistoryRecords = history
            };
        }

    }
}
