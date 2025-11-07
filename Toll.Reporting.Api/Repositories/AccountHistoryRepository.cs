using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.DTOs;
using Toll.Reporting.Api.Repositories;
using TollReportingSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toll.Reporting.Api.Repositories
{
    public class AccountHistoryRepository : IAccountHistoryRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountHistoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AccountHistoryDto> GetAccountHistoryAsync(string accountNumber)
        {
            // ✅ CASE 1: No filter → Return all accounts (no header)
            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                var allRecords = await (
                    from rtu in _context.RegisteredUserTopUps.AsNoTracking()
                    join ru in _context.RegisteredUsers.AsNoTracking() on rtu.RegisterUserId equals ru.RegisterUserId
                    join pm in _context.PaymentMethods.AsNoTracking() on rtu.PaymentMethodId equals pm.PaymentMethodId into pmGroup
                    from pm in pmGroup.DefaultIfEmpty()
                    join t in _context.Transactions.AsNoTracking() on ru.RegisterUserId equals t.RegisteredUserId into tGroup
                    from t in tGroup.DefaultIfEmpty()
                    join l in _context.Lanes.AsNoTracking() on t.LaneId equals l.LaneId into lGroup
                    from l in lGroup.DefaultIfEmpty()
                    join tt in _context.TransactionTypes.AsNoTracking() on t.TransactionTypeId equals tt.TransactionTypeId into ttGroup
                    from tt in ttGroup.DefaultIfEmpty()
                    join rui in _context.RegisteredUserIdentifiers.AsNoTracking() on ru.RegisterUserId equals rui.RegisteredUserId into ruiGroup
                    from rui in ruiGroup.DefaultIfEmpty()
                    select new AccountHistoryRecordDto
                    {
                        LaneName = l.LaneName ?? "Lekki-Ikoyi",
                        TransactionType = tt.Description ?? "N/A",
                        TransactionAmount = t.NettAmount != null ? Convert.ToDecimal(t.NettAmount) : 0,
                        TopUpAmount = rtu.Amount != null ? Convert.ToDecimal(rtu.Amount) : 0,
                        UserBalance = ru.Balance != null ? Convert.ToDecimal(ru.Balance) : 0,
                        PaymentMethod = pm.Description ?? "N/A",
                        TransactionDateTime = t.TransactionDateTime != null
                        ? t.TransactionDateTime
                        : rtu.RechargedOn,
                        RegisteredIdentifier = rui.RegisteredIdentifier ?? "N/A",
                        NumberPlate = rui.NumberPlateDetails ?? "N/A",
                        Description = $"Account: {ru.AccNr} ({ru.FirstName} {ru.LastName})"
                    }
                )
                .OrderByDescending(x => x.TransactionDateTime)
                .Take(5000)
                .ToListAsync();

                // Return all history (no header)
                return new AccountHistoryDto
                {
                    AccountHeader = null,
                    HistoryRecords = allRecords
                };
            }

            // ✅ CASE 2: Filtered by account number
            var accountHeader = await _context.RegisteredUsers
                .Where(ru => ru.AccNr.Trim() == accountNumber.Trim())
                .Select(ru => new AccountHeaderDto
                {
                    AccountNumber = ru.AccNr,
                    AccountHolder = (ru.CompanyName ?? ((ru.FirstName ?? "") + " " + (ru.LastName ?? ""))).Trim(),
                    AccountStatus = ru.IsActive == true ? "Active" : "Inactive",
                    AccountType = ru.IsPrepaid == true ? "Prepaid" : "Postpaid",
                    MobileNumber = ru.PrimaryContact ?? "N/A",
                    Email = ru.PrimaryEmail ?? "N/A",
                    AccountBalance = ru.Balance != null ? Convert.ToDecimal(ru.Balance) : 0
                })
                .FirstOrDefaultAsync();

            if (accountHeader == null)
                return new AccountHistoryDto
                {
                    AccountHeader = null,
                    HistoryRecords = new List<AccountHistoryRecordDto>()
                };

            // ✅ Transactions (Deducts)
            var transactionsQuery =
                from t in _context.Transactions.AsNoTracking()
                join ru in _context.RegisteredUsers.AsNoTracking() on t.RegisteredUserId equals ru.RegisterUserId
                join l in _context.Lanes.AsNoTracking() on t.LaneId equals l.LaneId into lGroup
                from l in lGroup.DefaultIfEmpty()
                join tt in _context.TransactionTypes.AsNoTracking() on t.TransactionTypeId equals tt.TransactionTypeId into ttGroup
                from tt in ttGroup.DefaultIfEmpty()
                join rui in _context.RegisteredUserIdentifiers.AsNoTracking() on ru.RegisterUserId equals rui.RegisteredUserId into ruiGroup
                from rui in ruiGroup.DefaultIfEmpty()
                where ru.AccNr.Trim() == accountNumber.Trim()
                select new AccountHistoryRecordDto
                {
                    LaneName = l.LaneName ?? "Lekki-Ikoyi",
                    TransactionType = tt.Description ?? "Lane Transaction",
                    TransactionAmount = t.NettAmount != null ? Convert.ToDecimal(t.NettAmount) : 0,
                    TopUpAmount = 0,
                    UserBalance = ru.Balance != null ? Convert.ToDecimal(ru.Balance) : 0,
                    PaymentMethod = "SmartCard",
                    TransactionDateTime = t.TransactionDateTime,
                    RegisteredIdentifier = rui.RegisteredIdentifier ?? "N/A",
                    NumberPlate = rui.NumberPlateDetails ?? "N/A",
                    Description = tt.Description ?? "Lane Transactions"
                };

            // ✅ Top-Ups (Credits)
            var topUpsQuery =
                from rtu in _context.RegisteredUserTopUps.AsNoTracking()
                join ru in _context.RegisteredUsers.AsNoTracking() on rtu.RegisterUserId equals ru.RegisterUserId
                join pm in _context.PaymentMethods.AsNoTracking() on rtu.PaymentMethodId equals pm.PaymentMethodId into pmGroup
                from pm in pmGroup.DefaultIfEmpty()
                where ru.AccNr.Trim() == accountNumber.Trim()
                select new AccountHistoryRecordDto
                {
                    LaneName = "Lekki-Ikoyi",
                    TransactionType = "Top-up",
                    TransactionAmount = 0,
                    TopUpAmount = rtu.Amount != null ? Convert.ToDecimal(rtu.Amount) : 0,
                    UserBalance = ru.Balance != null ? Convert.ToDecimal(ru.Balance) : 0,
                    PaymentMethod = pm.Description ?? "Back-Office",
                    TransactionDateTime = rtu.RechargedOn,
                    RegisteredIdentifier = "N/A",
                    NumberPlate = "N/A",
                    Description = "Account Top-up"
                };

            // ✅ Merge and return both
            var historyRecords = await transactionsQuery
                .Concat(topUpsQuery)
                .OrderBy(r => r.TransactionDateTime)
                .Take(2000)
                .ToListAsync();

            return new AccountHistoryDto
            {
                AccountHeader = accountHeader,
                HistoryRecords = historyRecords
            };
        }
    }
}
