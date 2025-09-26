using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class LaneLoginLogout
    {
        public long LaneLoginLogoutId { get; set; }
        public byte LaneId { get; set; }
        public long SystemUserId { get; set; }
        public byte ShiftId { get; set; }
        public DateTime ShiftDate { get; set; }
        public DateTime LoginAt { get; set; }
        public DateTime? LogOutAt { get; set; }
    }
}
