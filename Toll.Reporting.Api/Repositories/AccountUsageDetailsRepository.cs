using System;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<AccountUsageDetailsReportDto> GetAccountUsageDetailsAsync(
            DateTime startDate,
            DateTime endDate)
        {
            // Make end date inclusive of full day
            if (endDate.TimeOfDay == TimeSpan.Zero)
                endDate = endDate.AddDays(1).AddSeconds(-1);

            // ============================================================
            // 1️⃣ DETAILS QUERY — JOINS (RU + Movements + TopUp + Transaction)
            // ============================================================
            var detailsQuery =
                from ru in _context.RegisteredUsers.AsNoTracking()

                    // ACCOUNT MOVEMENTS
                join rum in _context.RegisterUserAccountMovements.AsNoTracking()
                    on ru.RegisterUserId equals rum.RegisterUserId into rumGroup
                from rum in rumGroup.DefaultIfEmpty()

                    // TOP UPS
                join rut in _context.RegisteredUserTopUps.AsNoTracking()
                    on ru.RegisterUserId equals rut.RegisterUserId into rutGroup
                from rut in rutGroup.DefaultIfEmpty()

                    // TRANSACTIONS
                join t in _context.Transactions.AsNoTracking()
                    on ru.RegisterUserId equals t.RegisteredUserId into tGroup
                from t in tGroup.DefaultIfEmpty()

                    // PAYMENT METHOD for TopUps
                join pm in _context.PaymentMethods.AsNoTracking()
                    on rut.PaymentMethodId equals pm.PaymentMethodId into pmGroup
                from pm in pmGroup.DefaultIfEmpty()

                    // FILTER RANGE
                where
                    (t != null &&
                     t.TransactionDateTime >= startDate &&
                     t.TransactionDateTime <= endDate)

                    ||
                    (t == null &&
                     rut != null &&
                     rut.RechargedOn >= startDate &&
                     rut.RechargedOn <= endDate)

                select new AccountUsageDetailsItemDto
                {
                    // ==========================================
                    // ACCOUNT INFO
                    // ==========================================
                    AccountNumber = ru.AccNr,

                    UserName = ((ru.FirstName ?? "") + " " + (ru.LastName ?? ""))
                        .Trim(),

                    VehicleRegNumber = ru.RegisteredUserIdentifiers
                        .Where(id => id.IsActive == true)
                        .Select(id => id.RegisteredIdentifier)
                        .FirstOrDefault() ?? string.Empty,

                    Status =
                        ru.IsActive == true ? "Active" :
                        ru.IsActive == false ? "Terminated" :
                        "Dormant",

                    // ==========================================
                    // BALANCES
                    // ==========================================
                    OpeningBalance = rum != null
                        ? (decimal)(rum.OpeningBalance ?? 0)
                        : 0m,

                    ClosingBalance = rum != null
                        ? (decimal)(rum.ClosingBalance ?? 0)
                        : 0m,

                    // ==========================================
                    // TRANSACTION DETAILS
                    // ==========================================
                    TransactionType = t != null && t.TransactionType != null
                        ? t.TransactionType.Description ?? ""
                        : "",

                    NettAmount = t != null ? (decimal)t.NettAmount : 0m,
                    DiscountValue = t != null ? (decimal)t.DiscountValue : 0m,
                    NominalTariff = t != null ? (decimal)t.NominalTariff : 0m,
                    VatAmount = t != null ? (decimal)t.VatAmout : 0m,

                    TransactionDateTime = t.TransactionDateTime,

                    LaneName = t.Lane.LaneName ?? "",

                    PaymentMethod = "",   // As discussed earlier

                    // ==========================================
                    // TOP UPS
                    // ==========================================
                    TopUpAmount = rut != null
                        ? (decimal)rut.Amount
                        : 0m,

                    TopUpMethod = pm != null
                        ? pm.Description ?? ""
                        : "",

                    TopUpDateTime = rut.RechargedOn
                };

            var details = await detailsQuery
                .OrderByDescending(x => x.TransactionDateTime ?? x.TopUpDateTime)
                .ToListAsync();

            // ============================================================
            // 2️⃣ SUMMARY CALCULATION
            // ============================================================
            var summary = new AccountUsageDetailsTotalsDto
            {
                TotalAccounts = details
                    .Where(x => !string.IsNullOrWhiteSpace(x.AccountNumber))
                    .Select(x => x.AccountNumber)
                    .Distinct()
                    .Count(),

                TotalOpeningBalance = details.Sum(x => x.OpeningBalance),
                TotalClosingBalance = details.Sum(x => x.ClosingBalance),

                TotalTopUp = details.Sum(x => x.TopUpAmount),
                TotalDeduct = details.Sum(x => x.NettAmount),

                TotalNett = details.Sum(x => x.NettAmount),
                TotalDiscount = details.Sum(x => x.DiscountValue),
                TotalNominal = details.Sum(x => x.NominalTariff),
                TotalVat = details.Sum(x => x.VatAmount),

                TotalTransactions = details.Count(x => x.NettAmount > 0),
                TotalTopUpCount = details.Count(x => x.TopUpAmount > 0),

                StartDate = startDate,
                EndDate = endDate
            };

            // ============================================================
            // 3️⃣ RETURN REPORT DTO
            // ============================================================
            return new AccountUsageDetailsReportDto
            {
                Summary = summary,
                Details = details
            };
        }
    }
}
