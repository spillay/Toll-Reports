using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TollReportingSystem.Data.Entities.Star
{
    [Table("OtherIncome", Schema = "star")]
    [Keyless]
    public class StarOtherIncome
    {
        public DateTime ReportDate { get; set; }

        public decimal? CashTopupAmount { get; set; }
        public decimal? DigitalTopupAmount { get; set; }
        public decimal? SwitchTopupAmount { get; set; }
        public decimal? NFCTopupAmount { get; set; }
        public decimal? BankDepositTopupAmount { get; set; }
        public decimal? TotalTopupAmount { get; set; }

        public decimal? TotalNettAmount { get; set; }
        public decimal? TotalActualAmount { get; set; }
        public decimal? TotalDeclaredAmount { get; set; }
        public decimal? ExpectedAmount { get; set; }

        public decimal? CashSurplusShortage { get; set; }
        public decimal? TotalOtherIncome { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }
}
