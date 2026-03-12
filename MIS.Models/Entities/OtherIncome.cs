using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class OtherIncome
{
    public DateOnly ReportDate { get; set; }

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public decimal CashTopupAmount { get; set; }

    public decimal DigitalTopupAmount { get; set; }

    public decimal SwitchTopupAmount { get; set; }

    public decimal NfctopupAmount { get; set; }

    public decimal BankDepositTopupAmount { get; set; }

    public decimal TotalTopupAmount { get; set; }

    public decimal TotalNettAmount { get; set; }

    public decimal TotalActualAmount { get; set; }

    public decimal TotalDeclaredAmount { get; set; }

    public decimal ExpectedAmount { get; set; }

    public decimal CashSurplusShortage { get; set; }

    public decimal TotalOtherIncome { get; set; }

    public DateTime CreatedDateTime { get; set; }
}
