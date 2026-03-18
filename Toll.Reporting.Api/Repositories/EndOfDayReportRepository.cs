using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.DTOs.EndOfDay;
using Toll.Reporting.Api.Repositories.Interfaces;
using TollReportingSystem.Data;

namespace Toll.Reporting.Api.Repositories
{
    public class EndOfDayReportRepository : IEndOfDayReportRepository
    {
        private readonly ApplicationDbContext _context;

        public EndOfDayReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EndOfDayReportDto?> GetEndOfDayAsync(DateTime startDate, DateTime endDate, int? shiftId = null)
        {
            var start = startDate.Date;
            var end = endDate.Date;

            // =========================================================
            // 1) THEORETICAL INCOME
            // =========================================================
            var theoreticalRaw = await _context.TheoreticalIncome
                .Where(t => t.ReportDate >= start && t.ReportDate <= end)
                .Select(t => new TheoreticalIncomeRowDto
                {
                    Metric = t.Metric ?? string.Empty,
                    ClassM = t.Class_M ?? 0,
                    Class1 = t.Class_I ?? 0,
                    Class2 = t.Class_II ?? 0,
                    Class3 = t.Class_III ?? 0,
                    Class4 = 0,   // entity does not contain Class_4
                    ClassD = 0,   // entity does not contain Class_D
                    Total = t.Total ?? 0
                })
                .ToListAsync();

            var theoreticalRows = theoreticalRaw
                .GroupBy(x => (x.Metric ?? string.Empty).Trim().ToLower())
                .Select(g => g.First())
                .ToList();

            var nominal = theoreticalRows.FirstOrDefault(x =>
                x.Metric.Equals("Nominal Tariff", StringComparison.OrdinalIgnoreCase));

            if (nominal != null)
            {
                nominal.ClassM = 25;
                nominal.Class1 = 50;
                nominal.Class2 = 200;
                nominal.Class3 = 500;
                nominal.Class4 = 0;
                nominal.ClassD = 0;
                nominal.Total = 0;
            }

            var totalTheoreticalIncome =
                theoreticalRows.FirstOrDefault(x =>
                    x.Metric.Equals("Traffic x Nominal Tariff", StringComparison.OrdinalIgnoreCase))?.Total ?? 0;

            // =========================================================
            // 2) DISCOUNTS
            // =========================================================
            var discounts = await _context.Discounts
                .Where(d => d.ReportDate >= start && d.ReportDate <= end)
                .GroupBy(_ => 1)
                .Select(g => new DiscountsDto
                {
                    Anonymous5 = new EndOfDayClassBreakdownDto
                    {
                        ClassM = g.Sum(x => x.Class_M_AnonymousAmount ?? 0),
                        Class1 = g.Sum(x => x.Class_I_AnonymousAmount ?? 0),
                        Class2 = g.Sum(x => x.Class_II_AnonymousAmount ?? 0),
                        Class3 = g.Sum(x => x.Class_III_AnonymousAmount ?? 0),
                        Class4 = 0,
                        ClassD = 0
                    },
                    Individual10 = new EndOfDayClassBreakdownDto
                    {
                        ClassM = g.Sum(x => x.Class_M_IndividualAmount ?? 0),
                        Class1 = g.Sum(x => x.Class_I_IndividualAmount ?? 0),
                        Class2 = g.Sum(x => x.Class_II_IndividualAmount ?? 0),
                        Class3 = g.Sum(x => x.Class_III_IndividualAmount ?? 0),
                        Class4 = 0,
                        ClassD = 0
                    },
                    Corporate10 = new EndOfDayClassBreakdownDto
                    {
                        ClassM = g.Sum(x => x.Class_M_CorporateAmount ?? 0),
                        Class1 = g.Sum(x => x.Class_I_CorporateAmount ?? 0),
                        Class2 = g.Sum(x => x.Class_II_CorporateAmount ?? 0),
                        Class3 = g.Sum(x => x.Class_III_CorporateAmount ?? 0),
                        Class4 = 0,
                        ClassD = 0
                    },
                    Staff100 = new EndOfDayClassBreakdownDto
                    {
                        ClassM = g.Sum(x => x.Class_M_StaffAmount ?? 0),
                        Class1 = g.Sum(x => x.Class_I_StaffAmount ?? 0),
                        Class2 = g.Sum(x => x.Class_II_StaffAmount ?? 0),
                        Class3 = g.Sum(x => x.Class_III_StaffAmount ?? 0),
                        Class4 = 0,
                        ClassD = 0
                    },
                    TotalDiscountCount = g.Sum(x => x.TotalDiscountCount ?? 0),
                    TotalDiscountAmount = g.Sum(x => x.TotalDiscountAmount ?? 0)
                })
                .SingleOrDefaultAsync() ?? new DiscountsDto();

            discounts.TotalDiscountedIncome = totalTheoreticalIncome - discounts.TotalDiscountAmount;

            // =========================================================
            // 3) EXEMPTS
            // =========================================================
            var exempts = await _context.Exempts
                .Where(e => e.ReportDate >= start && e.ReportDate <= end)
                .GroupBy(_ => 1)
                .Select(g => new ExemptsDto
                {
                    ClassM = g.Sum(x => x.Class_M_ExemptAmount ?? 0),
                    Class1 = g.Sum(x => x.Class_I_ExemptAmount ?? 0),
                    Class2 = g.Sum(x => x.Class_II_ExemptAmount ?? 0),
                    Class3 = g.Sum(x => x.Class_III_ExemptAmount ?? 0),
                    Class4 = 0,
                    ClassD = 0,
                    TotalExemptCount = g.Sum(x => x.TotalExemptCount ?? 0),
                    TotalExemptAmount = g.Sum(x => x.TotalExemptAmount ?? 0)
                })
                .SingleOrDefaultAsync() ?? new ExemptsDto();

            // =========================================================
            // 4) OTHER INCOME
            // =========================================================
            var otherIncome = await _context.OtherIncome
                .Where(o => o.ReportDate >= start && o.ReportDate <= end)
                .GroupBy(_ => 1)
                .Select(g => new OtherIncomeDto
                {
                    // derive from top-up channels since AccountPaymentsTopUp does not exist
                    AccountPaymentsTopUp =
                        g.Sum(x => x.CashTopupAmount ?? 0) +
                        g.Sum(x => x.DigitalTopupAmount ?? 0) +
                        g.Sum(x => x.SwitchTopupAmount ?? 0) +
                        g.Sum(x => x.NFCTopupAmount ?? 0) +
                        g.Sum(x => x.BankDepositTopupAmount ?? 0),

                    CashTopupAmount = g.Sum(x => x.CashTopupAmount ?? 0),
                    SwitchTopupAmount = g.Sum(x => x.SwitchTopupAmount ?? 0),
                    DigitalTopupAmount = g.Sum(x => x.DigitalTopupAmount ?? 0),
                    NFCTopupAmount = g.Sum(x => x.NFCTopupAmount ?? 0),
                    BankDepositTopupAmount = g.Sum(x => x.BankDepositTopupAmount ?? 0),
                    TotalTopupAmount = g.Sum(x => x.TotalTopupAmount ?? 0),

                    TotalNettAmount = g.Sum(x => x.TotalNettAmount ?? 0),
                    TotalActualAmount = g.Sum(x => x.TotalActualAmount ?? 0),
                    TotalDeclaredAmount = g.Sum(x => x.TotalDeclaredAmount ?? 0),
                    ExpectedAmount = g.Sum(x => x.ExpectedAmount ?? 0),

                    // derive since CashDeclaredSurplus does not exist
                    CashDeclaredSurplus = g.Sum(x => x.CashSurplusShortage ?? 0) > 0
                        ? g.Sum(x => x.CashSurplusShortage ?? 0)
                        : 0,

                    CashSurplusShortage = g.Sum(x => x.CashSurplusShortage ?? 0),
                    TotalOtherIncome = g.Sum(x => x.TotalOtherIncome ?? 0)
                })
                .SingleOrDefaultAsync() ?? new OtherIncomeDto();

            // =========================================================
            // 5) RECONCILIATION
            // =========================================================
            var reconciliation = await _context.Reconciliation
                .Where(r => r.ReportDate >= start && r.ReportDate <= end)
                .GroupBy(_ => 1)
                .Select(g => new ReconciliationDto
                {
                    CashDeclared = g.Sum(x => x.CashDeclared ?? 0),
                    CashBanked = g.Sum(x => x.CashBanked ?? 0),

                    // entity uses CashSurplusShortage, DTO uses CashBankedSurplusShortage
                    CashBankedSurplusShortage = g.Sum(x => x.CashSurplusShortage ?? 0),

                    SwitchAmount = g.Sum(x => x.SwitchAmount ?? 0),
                    DigitalAmount = g.Sum(x => x.DigitalAmount ?? 0),

                    PrePaidTotal = g.Sum(x => x.PrePaidTotal ?? 0),
                    SmartCardAmount = g.Sum(x => x.SmartCardAmount ?? 0),
                    ETCTagAmount = g.Sum(x => x.ETCTagAmount ?? 0),

                    OtherLaneTotal = g.Sum(x => x.OtherLaneTotal ?? 0),
                    ViolationAmount = g.Sum(x => x.ViolationAmount ?? 0),
                    ExemptionsAmount = g.Sum(x => x.ExemptionsAmount ?? 0),

                    CollectorDebt = g.Sum(x => x.CollectorDebt ?? 0),
                    CashShortages = g.Sum(x => x.CashShortages ?? 0),

                    TotalAccounted = g.Sum(x => x.TotalAccounted ?? 0),
                    Discrepancy = g.Sum(x => x.Discrepancy ?? 0)
                })
                .SingleOrDefaultAsync() ?? new ReconciliationDto();

            // =========================================================
            // 6) TOTALS
            // =========================================================
            var totalIncomeA = discounts.TotalDiscountedIncome + otherIncome.TotalOtherIncome;
            var totalAccountedB = reconciliation.TotalAccounted;
            var unreconciledDiscrepancy = totalIncomeA - totalAccountedB;

            var totals = new EndOfDayTotalsDto
            {
                TotalTheoreticalIncome = totalTheoreticalIncome,
                TotalExemptAmount = exempts.TotalExemptAmount,
                TotalDiscountAmount = discounts.TotalDiscountAmount,
                TotalDiscountedIncome = discounts.TotalDiscountedIncome,
                TotalIncomeA = totalIncomeA,
                TotalAccountedB = totalAccountedB,
                UnreconciledDiscrepancy = unreconciledDiscrepancy
            };

            return new EndOfDayReportDto
            {
                StartDate = start,
                EndDate = end,
                MonthLabel = start.ToString("MMMM yyyy"),
                IsOperationalDay = true,
                OperationalDayLabel = start.ToString("dd MMMM yyyy"),
                ShiftName = shiftId.HasValue ? $"Shift {shiftId.Value}" : "-All-",
                TheoreticalIncome = theoreticalRows,
                Exempts = exempts,
                Discounts = discounts,
                OtherIncome = otherIncome,
                Reconciliation = reconciliation,
                Totals = totals
            };
        }
    }
}