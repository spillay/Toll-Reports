using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class CollectorCashupShortagePayment
    {
        public long CollectorCashupShortagePaymentId { get; set; }
        public long CollectorCashupId { get; set; }
        public byte CashupShortagePaymentMethodId { get; set; }
        public long ReceivedById { get; set; }
        public DateTime ReceivedAt { get; set; }
        public double AmountReceived { get; set; }
        public long? CollectorCashupCashSurplusAllocatedToDiscrepancyId { get; set; }

        public virtual CashupShortagePaymentMethod CashupShortagePaymentMethod { get; set; }
        public virtual CollectorCashup CollectorCashup { get; set; }
        public virtual CollectorCashupCashSurplusAllocatedToDiscrepancy CollectorCashupCashSurplusAllocatedToDiscrepancy { get; set; }
        public virtual SystemUser ReceivedBy { get; set; }
    }
}
