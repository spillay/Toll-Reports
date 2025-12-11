using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.DTOs.EndOfDay;
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

        public async Task<EndOfDayReportDto?> GetEndOfDayAsync(DateTime startDate, DateTime endDate)
        {
            var start = startDate.Date;
            var end = endDate.Date;

            // ==============================
            // 1) THEORETICAL INCOME (Raw)
            // ==============================
            var theoreticalRaw = await _context.TheoreticalIncome
                .Where(t => t.ReportDate >= start && t.ReportDate <= end)
                .Select(t => new TheoreticalIncomeRowDto
                {
                    Metric = t.Metric,
                    ClassM = t.Class_M ?? 0,
                    ClassI = t.Class_I ?? 0,
                    ClassII = t.Class_II ?? 0,
                    ClassIII = t.Class_III ?? 0,
                    Total = t.Total ?? 0
                })
                .ToListAsync();

            // ==============================
            // GROUP theoretical income by metric
            // ==============================
            var theoreticalRows = theoreticalRaw
                .GroupBy(x => x.Metric.Trim().ToLower())
                .Select(g => g.First()) // prevent doubling
                .ToList();

            // Force-correct Nominal Tariff (constant table)
            var nominal = theoreticalRows
                .FirstOrDefault(x => x.Metric.Equals("Nominal Tariff", StringComparison.OrdinalIgnoreCase));

            if (nominal != null)
            {
                nominal.ClassM = 25;
                nominal.ClassI = 50;
                nominal.ClassII = 200;
                nominal.ClassIII = 500;
                nominal.Total = 0;
            }


            // ==============================
            // 2) DISCOUNTS
            // ==============================
            var discountAgg = await _context.Discounts
                .Where(d => d.ReportDate >= start && d.ReportDate <= end)
                .GroupBy(_ => 1)
                .Select(g => new DiscountsDto
                {
                    ClassM_AnonymousAmount = g.Sum(x => x.Class_M_AnonymousAmount ?? 0),
                    ClassI_AnonymousAmount = g.Sum(x => x.Class_I_AnonymousAmount ?? 0),
                    ClassII_AnonymousAmount = g.Sum(x => x.Class_II_AnonymousAmount ?? 0),
                    ClassIII_AnonymousAmount = g.Sum(x => x.Class_III_AnonymousAmount ?? 0),

                    ClassM_StaffAmount = g.Sum(x => x.Class_M_StaffAmount ?? 0),
                    ClassI_StaffAmount = g.Sum(x => x.Class_I_StaffAmount ?? 0),
                    ClassII_StaffAmount = g.Sum(x => x.Class_II_StaffAmount ?? 0),
                    ClassIII_StaffAmount = g.Sum(x => x.Class_III_StaffAmount ?? 0),

                    ClassM_IndividualAmount = g.Sum(x => x.Class_M_IndividualAmount ?? 0),
                    ClassI_IndividualAmount = g.Sum(x => x.Class_I_IndividualAmount ?? 0),
                    ClassII_IndividualAmount = g.Sum(x => x.Class_II_IndividualAmount ?? 0),
                    ClassIII_IndividualAmount = g.Sum(x => x.Class_III_IndividualAmount ?? 0),

                    ClassM_CorporateAmount = g.Sum(x => x.Class_M_CorporateAmount ?? 0),
                    ClassI_CorporateAmount = g.Sum(x => x.Class_I_CorporateAmount ?? 0),
                    ClassII_CorporateAmount = g.Sum(x => x.Class_II_CorporateAmount ?? 0),
                    ClassIII_CorporateAmount = g.Sum(x => x.Class_III_CorporateAmount ?? 0),

                    TotalDiscountCount = g.Sum(x => x.TotalDiscountCount ?? 0),
                    TotalDiscountAmount = g.Sum(x => x.TotalDiscountAmount ?? 0)
                })
                .SingleOrDefaultAsync() ?? new DiscountsDto();

            // ==============================
            // 3) EXEMPTS
            // ==============================
            var exemptsAgg = await _context.Exempts
                .Where(e => e.ReportDate >= start && e.ReportDate <= end)
                .GroupBy(_ => 1)
                .Select(g => new ExemptsDto
                {
                    ClassM_ExemptAmount = g.Sum(x => x.Class_M_ExemptAmount ?? 0),
                    ClassI_ExemptAmount = g.Sum(x => x.Class_I_ExemptAmount ?? 0),
                    ClassII_ExemptAmount = g.Sum(x => x.Class_II_ExemptAmount ?? 0),
                    ClassIII_ExemptAmount = g.Sum(x => x.Class_III_ExemptAmount ?? 0),

                    TotalExemptCount = g.Sum(x => x.TotalExemptCount ?? 0),
                    TotalExemptAmount = g.Sum(x => x.TotalExemptAmount ?? 0)
                })
                .SingleOrDefaultAsync() ?? new ExemptsDto();

            // ==============================
            // 4) OTHER INCOME
            // ==============================
            var otherIncomeAgg = await _context.OtherIncome
                .Where(o => o.ReportDate >= start && o.ReportDate <= end)
                .GroupBy(_ => 1)
                .Select(g => new OtherIncomeDto
                {
                    CashTopupAmount = g.Sum(x => x.CashTopupAmount ?? 0),
                    DigitalTopupAmount = g.Sum(x => x.DigitalTopupAmount ?? 0),
                    SwitchTopupAmount = g.Sum(x => x.SwitchTopupAmount ?? 0),
                    NFCTopupAmount = g.Sum(x => x.NFCTopupAmount ?? 0),
                    BankDepositTopupAmount = g.Sum(x => x.BankDepositTopupAmount ?? 0),
                    TotalTopupAmount = g.Sum(x => x.TotalTopupAmount ?? 0),

                    TotalNettAmount = g.Sum(x => x.TotalNettAmount ?? 0),
                    TotalActualAmount = g.Sum(x => x.TotalActualAmount ?? 0),
                    TotalDeclaredAmount = g.Sum(x => x.TotalDeclaredAmount ?? 0),
                    ExpectedAmount = g.Sum(x => x.ExpectedAmount ?? 0),
                    CashSurplusShortage = g.Sum(x => x.CashSurplusShortage ?? 0),
                    TotalOtherIncome = g.Sum(x => x.TotalOtherIncome ?? 0)
                })
                .SingleOrDefaultAsync() ?? new OtherIncomeDto();

            // ==============================
            // 5) RECONCILIATION
            // ==============================
            var reconAgg = await _context.Reconciliation
                .Where(r => r.ReportDate >= start && r.ReportDate <= end)
                .GroupBy(_ => 1)
                .Select(g => new ReconciliationDto
                {
                    CashDeclared = g.Sum(x => x.CashDeclared ?? 0),
                    CashBanked = g.Sum(x => x.CashBanked ?? 0),
                    CashSurplusShortage = g.Sum(x => x.CashSurplusShortage ?? 0),

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

            // ==============================
            // 6) TOTALS (A, B, A-B)
            // ==============================
            var totalA = otherIncomeAgg.ExpectedAmount + otherIncomeAgg.TotalOtherIncome;
            var totalB = reconAgg.TotalAccounted;

            var totals = new EndOfDayTotalsDto
            {
                TotalIncomeA = totalA,
                TotalAccountedB = totalB,
                UnreconciledDiscrepancy = totalA - totalB
            };

            // ==============================
            // FINAL DTO
            // ==============================
            return new EndOfDayReportDto
            {
                StartDate = start,
                EndDate = end,
                TheoreticalIncome = theoreticalRows,
                Discounts = discountAgg,
                Exempts = exemptsAgg,
                OtherIncome = otherIncomeAgg,
                Reconciliation = reconAgg,
                Totals = totals
            };
        }
    }
}
