using System;
using System.Collections.Generic;

namespace MIS.Web.Models.EndOfDay
{
    public class EndOfDayReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<TheoreticalIncomeRowViewModel> TheoreticalIncome { get; set; }
            = new();

        public DiscountsViewModel Discounts { get; set; } = new();
        public ExemptsViewModel Exempts { get; set; } = new();
        public OtherIncomeViewModel OtherIncome { get; set; } = new();
        public ReconciliationViewModel Reconciliation { get; set; } = new();
        public EndOfDayTotalsViewModel Totals { get; set; } = new();
    }

    public class TheoreticalIncomeRowViewModel
    {
        public string Metric { get; set; } = string.Empty;
        public decimal ClassM { get; set; }
        public decimal ClassI { get; set; }
        public decimal ClassII { get; set; }
        public decimal ClassIII { get; set; }
        public decimal Total { get; set; }
    }

    public class DiscountsViewModel
    {
        public decimal ClassM_AnonymousAmount { get; set; }
        public decimal ClassI_AnonymousAmount { get; set; }
        public decimal ClassII_AnonymousAmount { get; set; }
        public decimal ClassIII_AnonymousAmount { get; set; }

        public decimal ClassM_StaffAmount { get; set; }
        public decimal ClassI_StaffAmount { get; set; }
        public decimal ClassII_StaffAmount { get; set; }
        public decimal ClassIII_StaffAmount { get; set; }

        public decimal ClassM_IndividualAmount { get; set; }
        public decimal ClassI_IndividualAmount { get; set; }
        public decimal ClassII_IndividualAmount { get; set; }
        public decimal ClassIII_IndividualAmount { get; set; }

        public decimal ClassM_CorporateAmount { get; set; }
        public decimal ClassI_CorporateAmount { get; set; }
        public decimal ClassII_CorporateAmount { get; set; }
        public decimal ClassIII_CorporateAmount { get; set; }

        public int TotalDiscountCount { get; set; }
        public decimal TotalDiscountAmount { get; set; }
    }

    public class ExemptsViewModel
    {
        public decimal ClassM_ExemptAmount { get; set; }
        public decimal ClassI_ExemptAmount { get; set; }
        public decimal ClassII_ExemptAmount { get; set; }
        public decimal ClassIII_ExemptAmount { get; set; }
        public decimal TotalExemptAmount { get; set; }
    }

    public class OtherIncomeViewModel
    {
        public decimal CashTopupAmount { get; set; }
        public decimal DigitalTopupAmount { get; set; }
        public decimal SwitchTopupAmount { get; set; }
        public decimal NFCTopupAmount { get; set; }
        public decimal BankDepositTopupAmount { get; set; }
        public decimal TotalTopupAmount { get; set; }

        public decimal TotalNettAmount { get; set; }
        public decimal TotalActualAmount { get; set; }
        public decimal TotalDeclaredAmount { get; set; }
        public decimal ExpectedAmount { get; set; }
        public decimal CashSurplusShortage { get; set; }
        public decimal TotalOtherIncome { get; set; }
    }

    public class ReconciliationViewModel
    {
        public decimal CashDeclared { get; set; }
        public decimal CashBanked { get; set; }
        public decimal CashSurplusShortage { get; set; }
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
        public decimal TotalIncomeA { get; set; }
        public decimal TotalAccountedB { get; set; }
        public decimal UnreconciledDiscrepancy { get; set; }
    }
}
