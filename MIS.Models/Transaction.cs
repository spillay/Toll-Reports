using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class Transaction
    {
        public Transaction()
        {
            TransactionClassCorrections = new HashSet<TransactionClassCorrection>();
            TransactionCreditNotes = new HashSet<TransactionCreditNote>();
            TransactionImages = new HashSet<TransactionImage>();
        }

        public byte LaneId { get; set; }
        public long TransactionNumber { get; set; }
        public byte TransactionTypeId { get; set; }
        public byte DiscountTypeId { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public byte ShiftId { get; set; }
        public DateTime ShiftDate { get; set; }
        public long? SystemUserId { get; set; }
        public byte ManualTollClassId { get; set; }
        public int TariffPlanId { get; set; }
        public byte CurrencyId { get; set; }
        public double NominalTariff { get; set; }
        public double DiscountValue { get; set; }
        public double DiscountPercentage { get; set; }
        public double NettAmount { get; set; }
        public double VatAmout { get; set; }
        public string InvoiceReceiptPrefix { get; set; }
        public long? InvoiceNo { get; set; }
        public long? ReceiptNo { get; set; }
        public byte? AutomaticTollClassId { get; set; }
        public double? AutomaticAmount { get; set; }
        public double? ForeignCurrencyReceived { get; set; }
        public double? ExchangeRateUsed { get; set; }
        public double? ChangeInLocalCurrency { get; set; }
        public long? RegisteredUserId { get; set; }
        public string RegisteredIdentifier { get; set; }
        public byte? RegisteredTollClassId { get; set; }
        public string CardNumber { get; set; }
        public byte? ActualTollClassId { get; set; }
        public double? ActualAmount { get; set; }
        public byte[] RowVersion { get; set; }
        public long? AllocatedToCollectorCashupId { get; set; }

        public virtual TollClass ActualTollClass { get; set; }
        public virtual CollectorCashup AllocatedToCollectorCashup { get; set; }
        public virtual TollClass AutomaticTollClass { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual DiscountType DiscountType { get; set; }
        public virtual Lane Lane { get; set; }
        public virtual TollClass ManualTollClass { get; set; }
        public virtual TollClass RegisteredTollClass { get; set; }
        public virtual RegisteredUser RegisteredUser { get; set; }
        public virtual Shift Shift { get; set; }
        public virtual SystemUser SystemUser { get; set; }
        public virtual TariffPlan TariffPlan { get; set; }
        public virtual TransactionType TransactionType { get; set; }
        public virtual ICollection<TransactionClassCorrection> TransactionClassCorrections { get; set; }
        public virtual ICollection<TransactionCreditNote> TransactionCreditNotes { get; set; }
        public virtual ICollection<TransactionImage> TransactionImages { get; set; }
    }
}
