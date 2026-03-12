using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class CollectorCashup
{
    public long CollectorCashupId { get; set; }

    public DateOnly ShiftDate { get; set; }

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

    public double TotalInvalidTimeouts { get; set; }

    public double ShortagesReceived { get; set; }

    public double OutstandingAmount { get; set; }

    public double TotalCashSurplus { get; set; }

    public double TotalTransactionSurplus { get; set; }

    public double TotalSurplus { get; set; }

    public double TotalZarsuplus { get; set; }

    public double TotalUsdsurplus { get; set; }

    public virtual ICollection<CollectorCashDeclaration> CollectorCashDeclarations { get; set; } = new List<CollectorCashDeclaration>();

    public virtual ICollection<CollectorCashupCashSurplusAllocatedToDiscrepancy> CollectorCashupCashSurplusAllocatedToDiscrepancies { get; set; } = new List<CollectorCashupCashSurplusAllocatedToDiscrepancy>();

    public virtual ICollection<CollectorCashupReprocess> CollectorCashupReprocesses { get; set; } = new List<CollectorCashupReprocess>();

    public virtual ICollection<CollectorCashupShortagePayment> CollectorCashupShortagePayments { get; set; } = new List<CollectorCashupShortagePayment>();

    public virtual Shift Shift { get; set; } = null!;

    public virtual SystemUser SystemUser { get; set; } = null!;

    public virtual ICollection<Transaction1> Transaction1s { get; set; } = new List<Transaction1>();

    public virtual ICollection<TransactionClassCorrection> TransactionClassCorrections { get; set; } = new List<TransactionClassCorrection>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
