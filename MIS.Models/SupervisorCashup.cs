using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class SupervisorCashup
    {
        public long SupervisorCashupId { get; set; }
        public DateTime ShiftDate { get; set; }
        public byte ShiftId { get; set; }
        public long SystemUserId { get; set; }
        public double TotalLocalCurrencyDeclared { get; set; }
        public double TotalForeignCurrenyDeclared { get; set; }
        public double TotalZar { get; set; }
        public double TotalUsd { get; set; }
        public double TotalDeclared { get; set; }
        public DateTime CashedUpAt { get; set; }
        public long? VerifiedById { get; set; }
        public double? VerifiedTotalLocalCurrencyDeclared { get; set; }
        public double? VerifiedTotalForeignCurrenyDeclared { get; set; }
        public double? VerifiedTotalZar { get; set; }
        public double? VerifiedTotalUsd { get; set; }
        public double? VerifiedTotalDeclared { get; set; }
        public double? VarianceTotalLocalCurrencyDeclared { get; set; }
        public double? VarianceTotalForeignCurrenyDeclared { get; set; }
        public double? VarianceTotalZar { get; set; }
        public double? VarianceTotalUsd { get; set; }
        public double? VarianceTotalDeclared { get; set; }
        public DateTime? VerifiedAt { get; set; }

        public virtual Shift Shift { get; set; }
        public virtual SystemUser SystemUser { get; set; }
        public virtual SystemUser VerifiedBy { get; set; }
    }
}
