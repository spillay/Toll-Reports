using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class TransactionClassCorrection
    {
        public long TransactionClassCorrectionId { get; set; }
        public byte LaneId { get; set; }
        public long TransactionNumber { get; set; }
        public byte ClassCorrectionTypeId { get; set; }
        public bool ClassCorrected { get; set; }
        public long CorrectedById { get; set; }
        public byte? AllocatedToId { get; set; }
        public double AllocatedAmount { get; set; }
        public byte? CorrectedClassId { get; set; }
        public bool? InvalidExempt { get; set; }
        public bool? InvalidDiscount { get; set; }
        public byte? ExemptTypeId { get; set; }
        public long? AllocatedToCollectorCashupId { get; set; }

        public virtual AllocatedTo AllocatedTo { get; set; }
        public virtual CollectorCashup AllocatedToCollectorCashup { get; set; }
        public virtual ClassCorrectionType ClassCorrectionType { get; set; }
        public virtual TollClass CorrectedClass { get; set; }
        public virtual ExemptType ExemptType { get; set; }
        public virtual Lane Lane { get; set; }
        public virtual Transaction Transaction { get; set; }
    }
}
