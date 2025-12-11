using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TollReportingSystem.Data.Entities.Star
{
    [Table("Reconciliation", Schema = "star")]
    [Keyless]
    public class StarReconciliation
    {
        public DateTime ReportDate { get; set; }

        public decimal? CashDeclared { get; set; }
        public decimal? CashBanked { get; set; }
        public decimal? CashSurplusShortage { get; set; }

        public decimal? SwitchAmount { get; set; }
        public decimal? DigitalAmount { get; set; }

        public decimal? PrePaidTotal { get; set; }
        public decimal? SmartCardAmount { get; set; }
        public decimal? ETCTagAmount { get; set; }

        public decimal? OtherLaneTotal { get; set; }
        public decimal? ViolationAmount { get; set; }
        public decimal? ExemptionsAmount { get; set; }

        public decimal? CollectorDebt { get; set; }
        public decimal? CashShortages { get; set; }

        public decimal? TotalAccounted { get; set; }
        public decimal? Discrepancy { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }
}
