using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class Transaction
{
    public byte LaneId { get; set; }

    public long TransactionNumber { get; set; }

    public byte TransactionTypeId { get; set; }

    public byte DiscountTypeId { get; set; }

    public DateTime TransactionDateTime { get; set; }

    public byte ShiftId { get; set; }

    public DateOnly ShiftDate { get; set; }

    public long? SystemUserId { get; set; }

    public byte ManualTollClassId { get; set; }

    public int TariffPlanId { get; set; }

    public byte CurrencyId { get; set; }

    public double NominalTariff { get; set; }

    public double DiscountValue { get; set; }

    public double DiscountPercentage { get; set; }

    public double NettAmount { get; set; }

    public double VatAmout { get; set; }

    public string InvoiceReceiptPrefix { get; set; } = null!;

    public long? InvoiceNo { get; set; }

    public long? ReceiptNo { get; set; }

    public byte? AutomaticTollClassId { get; set; }

    public double? AutomaticAmount { get; set; }

    public double? ForeignCurrencyReceived { get; set; }

    public double? ExchangeRateUsed { get; set; }

    public double? ChangeInLocalCurrency { get; set; }

    public long? RegisteredUserId { get; set; }

    public string? RegisteredIdentifier { get; set; }

    public byte? RegisteredTollClassId { get; set; }

    public string? CardNumber { get; set; }

    public byte? ActualTollClassId { get; set; }

    public double? ActualAmount { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public long? AllocatedToCollectorCashupId { get; set; }

    public virtual TollClass? ActualTollClass { get; set; }

    public virtual CollectorCashup? AllocatedToCollectorCashup { get; set; }

    public virtual TollClass? AutomaticTollClass { get; set; }

    public virtual Currency Currency { get; set; } = null!;

    public virtual DiscountType DiscountType { get; set; } = null!;

    public virtual Lane Lane { get; set; } = null!;

    public virtual TollClass ManualTollClass { get; set; } = null!;

    public virtual TollClass? RegisteredTollClass { get; set; }

    public virtual RegisteredUser? RegisteredUser { get; set; }

    public virtual Shift Shift { get; set; } = null!;

    public virtual TariffPlan TariffPlan { get; set; } = null!;

    public virtual ICollection<TransactionClassCorrection> TransactionClassCorrections { get; set; } = new List<TransactionClassCorrection>();

    public virtual ICollection<TransactionCreditNote> TransactionCreditNotes { get; set; } = new List<TransactionCreditNote>();

    public virtual ICollection<TransactionImage> TransactionImages { get; set; } = new List<TransactionImage>();

    public virtual TransactionType TransactionType { get; set; } = null!;
}
