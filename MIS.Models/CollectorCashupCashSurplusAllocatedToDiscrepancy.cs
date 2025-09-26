using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class CollectorCashupCashSurplusAllocatedToDiscrepancy
    {
        public CollectorCashupCashSurplusAllocatedToDiscrepancy()
        {
            CollectorCashupShortagePayments = new HashSet<CollectorCashupShortagePayment>();
        }

        public long CollectorCashupCashSurplusAllocatedToDiscrepancyId { get; set; }
        public long CollectorCashUpId { get; set; }
        public double AmountAllocated { get; set; }

        public virtual CollectorCashup CollectorCashUp { get; set; }
        public virtual ICollection<CollectorCashupShortagePayment> CollectorCashupShortagePayments { get; set; }
    }
}
