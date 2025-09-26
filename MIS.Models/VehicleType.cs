using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class VehicleType
    {
        public VehicleType()
        {
            RegisteredUserIdentifiers = new HashSet<RegisteredUserIdentifier>();
        }

        public int VehicleTypeId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<RegisteredUserIdentifier> RegisteredUserIdentifiers { get; set; }
    }
}
