using System;
using System.Collections.Generic;

namespace MIS.Web.Models.EndOfDay
{
    public class EndOfDayReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string MonthLabel { get; set; } = string.Empty;
        public bool IsOperationalDay { get; set; }
        public string OperationalDayLabel { get; set; } = string.Empty;
        public string ShiftName { get; set; } = "-All-";

        public List<TheoreticalIncomeRowViewModel> TheoreticalIncome { get; set; } = new();
        public List<EndOfDayReportRowViewModel> Rows { get; set; } = new();

        public ExemptsViewModel Exempts { get; set; } = new();
        public DiscountsViewModel Discounts { get; set; } = new();
        public OtherIncomeViewModel OtherIncome { get; set; } = new();
        public ReconciliationViewModel Reconciliation { get; set; } = new();
        public EndOfDayTotalsViewModel Totals { get; set; } = new();
    }

    public class EndOfDayReportRowViewModel
    {
        public string Col1 { get; set; } = string.Empty;
        public string Col2 { get; set; } = string.Empty;
        public string Col3 { get; set; } = string.Empty;
        public string Col4 { get; set; } = string.Empty;
        public string Col5 { get; set; } = string.Empty;
        public string Col6 { get; set; } = string.Empty;
        public string Col7 { get; set; } = string.Empty;
        public string Col8 { get; set; } = string.Empty;
    }

    public class TheoreticalIncomeRowViewModel
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

    public class ExemptsViewModel
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

    public class DiscountsViewModel
    {
        public EndOfDayClassBreakdownViewModel Anonymous5 { get; set; } = new();
        public EndOfDayClassBreakdownViewModel Individual10 { get; set; } = new();
        public EndOfDayClassBreakdownViewModel Corporate10 { get; set; } = new();
        public EndOfDayClassBreakdownViewModel Staff100 { get; set; } = new();

        public int TotalDiscountCount { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal TotalDiscountedIncome { get; set; }
    }

    public class OtherIncomeViewModel
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

    public class ReconciliationViewModel
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

    public class EndOfDayTotalsViewModel
    {
        public decimal TotalTheoreticalIncome { get; set; }
        public decimal TotalExemptAmount { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal TotalDiscountedIncome { get; set; }

        public decimal TotalIncomeA { get; set; }
        public decimal TotalAccountedB { get; set; }
        public decimal UnreconciledDiscrepancy { get; set; }
    }

    public class EndOfDayClassBreakdownViewModel
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
