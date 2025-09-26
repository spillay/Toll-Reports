using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class Suburb
    {
        public Suburb()
        {
            RegisteredUserAddresses = new HashSet<RegisteredUserAddress>();
        }

        public int SuburbId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<RegisteredUserAddress> RegisteredUserAddresses { get; set; }
    }
}
