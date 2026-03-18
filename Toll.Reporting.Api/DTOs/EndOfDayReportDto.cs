using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.DTOs.EndOfDay
{
    public class EndOfDayReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string MonthLabel { get; set; } = string.Empty;
        public bool IsOperationalDay { get; set; }
        public string OperationalDayLabel { get; set; } = string.Empty;
        public string ShiftName { get; set; } = "-All-";

        public List<TheoreticalIncomeRowDto> TheoreticalIncome { get; set; } = new();

        public ExemptsDto Exempts { get; set; } = new();
        public DiscountsDto Discounts { get; set; } = new();
        public OtherIncomeDto OtherIncome { get; set; } = new();
        public ReconciliationDto Reconciliation { get; set; } = new();
        public EndOfDayTotalsDto Totals { get; set; } = new();
    }

    public class TheoreticalIncomeRowDto
    {
        public string Metric { get; set; } = string.Empty;

        public decimal ClassM { get; set; }
        public decimal Class1 { get; set; }
        public decimal Class2 { get; set; }
        public decimal Class3 { get; set; }
        public decimal Class4 { get; set; }
        public decimal ClassD { get; set; }

        public decimal Total { get; set; }
    }

    public class ExemptsDto
    {
        public decimal ClassM { get; set; }
        public decimal Class1 { get; set; }
        public decimal Class2 { get; set; }
        public decimal Class3 { get; set; }
        public decimal Class4 { get; set; }
        public decimal ClassD { get; set; }

        public int TotalExemptCount { get; set; }
        public decimal TotalExemptAmount { get; set; }
    }

    public class DiscountsDto
    {
        public EndOfDayClassBreakdownDto Anonymous5 { get; set; } = new();
        public EndOfDayClassBreakdownDto Individual10 { get; set; } = new();
        public EndOfDayClassBreakdownDto Corporate10 { get; set; } = new();
        public EndOfDayClassBreakdownDto Staff100 { get; set; } = new();

        public int TotalDiscountCount { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal TotalDiscountedIncome { get; set; }
    }

    public class OtherIncomeDto
    {
        public decimal AccountPaymentsTopUp { get; set; }

        public decimal CashTopupAmount { get; set; }
        public decimal SwitchTopupAmount { get; set; }
        public decimal DigitalTopupAmount { get; set; }
        public decimal NFCTopupAmount { get; set; }
        public decimal BankDepositTopupAmount { get; set; }

        public decimal TotalTopupAmount { get; set; }

        public decimal TotalNettAmount { get; set; }
        public decimal TotalActualAmount { get; set; }
        public decimal TotalDeclaredAmount { get; set; }
        public decimal ExpectedAmount { get; set; }

        public decimal CashDeclaredSurplus { get; set; }
        public decimal CashSurplusShortage { get; set; }

        public decimal TotalOtherIncome { get; set; }
    }

    public class ReconciliationDto
    {
        public decimal CashDeclared { get; set; }
        public decimal CashBanked { get; set; }
        public decimal CashBankedSurplusShortage { get; set; }

        public decimal SwitchAmount { get; set; }
        public decimal DigitalAmount { get; set; }

        public decimal PrePaidTotal { get; set; }
        public decimal SmartCardAmount { get; set; }
        public decimal ETCTagAmount { get; set; }

        public decimal OtherLaneTotal { get; set; }
        public decimal ViolationAmount { get; set; }
        public decimal ExemptionsAmount { get; set; }

        public decimal CollectorDebt { get; set; }
        public decimal CashShortages { get; set; }

        public decimal TotalAccounted { get; set; }
        public decimal Discrepancy { get; set; }
    }

    public class EndOfDayTotalsDto
    {
        public decimal TotalTheoreticalIncome { get; set; }
        public decimal TotalExemptAmount { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal TotalDiscountedIncome { get; set; }

        public decimal TotalIncomeA { get; set; }
        public decimal TotalAccountedB { get; set; }
        public decimal UnreconciledDiscrepancy { get; set; }
    }

    public class EndOfDayClassBreakdownDto
    {
        public decimal ClassM { get; set; }
        public decimal Class1 { get; set; }
        public decimal Class2 { get; set; }
        public decimal Class3 { get; set; }
        public decimal Class4 { get; set; }
        public decimal ClassD { get; set; }

        public decimal Total =>
            ClassM + Class1 + Class2 + Class3 + Class4 + ClassD;
    }
}