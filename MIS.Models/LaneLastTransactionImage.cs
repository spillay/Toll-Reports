using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class LaneLastTransactionImage
    {
        public byte LaneId { get; set; }
        public long LastLaneTransactionImageId { get; set; }

        public virtual Lane Lane { get; set; }
    }
}
