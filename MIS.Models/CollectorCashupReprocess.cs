using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class CollectorCashupReprocess
    {
        public long CollectorCashupReprocessId { get; set; }
        public long CollectorCashupId { get; set; }
        public DateTime ShiftDate { get; set; }
        public byte ShiftId { get; set; }
        public long SystemUserId { get; set; }
        public byte TotalNumberOfCashDeclarations { get; set; }
        public double TotalLocalCurrencyDeclared { get; set; }
        public double TotalForeignCurrenyDeclared { get; set; }
        public double TotalZardeclared { get; set; }
        public double TotalUsddeclared { get; set; }
        public double TotalDeclared { get; set; }
        public double TotalDeclaredAllocatedToDiscrepancy { get; set; }
        public double TotalChangeGivenForeignCurrency { get; set; }
        public double TotalCashTransactions { get; set; }
        public double TotalZarreceived { get; set; }
        public double TotalUsdreceived { get; set; }
        public double TotalMisclassification { get; set; }
        public double TotalInvalidExempts { get; set; }
        public double TotalViolations { get; set; }
        public double TotalCashShortage { get; set; }
        public double TotalTransactionShortages { get; set; }
        public double TotalUsdshortages { get; set; }
        public double TotalZarshortages { get; set; }
        public double TotalShortages { get; set; }
        public DateTime CashedUpAt { get; set; }
        public long RerunBy { get; set; }
        public double TotalInvalidTimeouts { get; set; }
        public double OutstandingAmount { get; set; }
        public double ShortagesReceived { get; set; }
        public double TotalCashSurplus { get; set; }
        public double TotalTransactionSurplus { get; set; }
        public double TotalSurplus { get; set; }
        public double TotalZarsuplus { get; set; }
        public double TotalUsdsurplus { get; set; }

        public virtual CollectorCashup CollectorCashup { get; set; }
    }
}
