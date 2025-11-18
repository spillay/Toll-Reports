using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<AccountUsageSummaryReportDto> GetSummaryAsync(DateTime startDate, DateTime endDate)
        {
            // DETAILS
            var items = await (
                from ru in _context.RegisteredUsers

                join rum in _context.RegisterUserAccountMovements
                    on ru.RegisterUserId equals rum.RegisterUserId into rumGroup
                from rum in rumGroup.DefaultIfEmpty()

                join rut in _context.RegisteredUserTopUps
                    on ru.RegisterUserId equals rut.RegisterUserId into rutGroup
                from rut in rutGroup.DefaultIfEmpty()

                join t in _context.Transactions
                    on ru.RegisterUserId equals t.RegisteredUserId into tGroup
                from t in tGroup.DefaultIfEmpty()

                where
                    (t != null &&
                     t.TransactionDateTime >= startDate &&
                     t.TransactionDateTime <= endDate)
                    ||
                    (t == null &&
                     rut != null &&
                     rut.RechargedOn >= startDate &&
                     rut.RechargedOn <= endDate)

                group new { ru, rum, rut, t } by new
                {
                    ru.RegisterUserId,
                    ru.AccNr,
                    ru.IsActive,
                    rum.OpeningBalance,
                    rum.ClosingBalance
                } into g

                select new AccountUsageSummaryItemDto
                {
                    AccountNumber = g.Key.AccNr,
                    AccountStatus =
                        g.Key.IsActive == true ? "Active" :
                        g.Key.IsActive == false ? "Terminated" :
                        "Dormant",

                    OpeningBalance = (decimal)(g.Key.OpeningBalance ?? 0),
                    ClosingBalance = (decimal)(g.Key.ClosingBalance ?? 0),

                    LaneTransactionCount = g.Count(x => x.t != null),
                    LaneTransactionValue = (decimal)g.Sum(x => x.t != null ? x.t.NettAmount : 0),

                    LaneDiscountCount = g.Count(x => x.t != null && x.t.DiscountValue > 0),
                    LaneDiscountValue = (decimal)g.Sum(x => x.t != null ? x.t.DiscountValue : 0),

                    ReceiptTopUp = (decimal)g.Sum(x => x.rut != null ? x.rut.Amount : 0),
                    ReceiptDeposit = 0,

                    PaymentFees = (decimal)g.Sum(x => x.t != null ? x.t.VatAmout : 0),
                    PaymentRefunds = 0,

                    RefundAccount = 0,
                    RefundDeposit = 0
                }
            ).ToListAsync();


            // TOTALS
            var totals = new AccountUsageSummaryTotalsDto
            {
                TotalAccounts = await _context.RegisteredUsers.CountAsync(),
                Active = await _context.RegisteredUsers.CountAsync(x => x.IsActive == true),
                Terminated = await _context.RegisteredUsers.CountAsync(x => x.IsActive == false),
                Dormant = await _context.RegisteredUsers.CountAsync(x => x.IsActive == null),

                TotalEIdDevices = await _context.RegisteredUserIdentifiers.CountAsync(),
                TotalEtcTags = await _context.RegisteredUserIdentifiers
                    .CountAsync(x => x.IdentifierType.Description == "ETC"),
                TotalSmartCards = await _context.RegisteredUserIdentifiers
                    .CountAsync(x => x.IdentifierType.Description == "SmartCard"),

                StartDate = startDate,
                EndDate = endDate
            };

            return new AccountUsageSummaryReportDto
            {
                Summary = totals,
                Items = items
            };
        }
    }
}
