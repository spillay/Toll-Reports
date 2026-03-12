using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class CollectorCashupCashSurplusAllocatedToDiscrepancy
{
    public long CollectorCashupCashSurplusAllocatedToDiscrepancyId { get; set; }

    public long CollectorCashUpId { get; set; }

    public double AmountAllocated { get; set; }

    public virtual CollectorCashup CollectorCashUp { get; set; }

    public virtual ICollection<CollectorCashupShortagePayment> CollectorCashupShortagePayments { get; set; } = new List<CollectorCashupShortagePayment>();
}
