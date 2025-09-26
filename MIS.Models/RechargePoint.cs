using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class RechargePoint
    {
        public RechargePoint()
        {
            RegisteredUserTopUps = new HashSet<RegisteredUserTopUp>();
        }

        public byte RechargePointId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<RegisteredUserTopUp> RegisteredUserTopUps { get; set; }
    }
}
