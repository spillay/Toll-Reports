using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class CollectorCashupShortagePayment
{
    public long CollectorCashupShortagePaymentId { get; set; }

    public long CollectorCashupId { get; set; }

    public byte CashupShortagePaymentMethodId { get; set; }

    public long ReceivedById { get; set; }

    public DateTime ReceivedAt { get; set; }

    public double AmountReceived { get; set; }

    public long? CollectorCashupCashSurplusAllocatedToDiscrepancyId { get; set; }

    public virtual CashupShortagePaymentMethod CashupShortagePaymentMethod { get; set; } = null!;

    public virtual CollectorCashup CollectorCashup { get; set; } = null!;

    public virtual CollectorCashupCashSurplusAllocatedToDiscrepancy? CollectorCashupCashSurplusAllocatedToDiscrepancy { get; set; }

    public virtual SystemUser ReceivedBy { get; set; } = null!;
}
