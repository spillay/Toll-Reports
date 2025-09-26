using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class CollectorShiftAssignment
    {
        public long SystemUserId { get; set; }
        public DateTime ShiftDate { get; set; }
        public byte ShiftId { get; set; }
        public byte ShiftStatusId { get; set; }

        public virtual Shift Shift { get; set; }
        public virtual SystemUser SystemUser { get; set; }
    }
}
