using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class Province
    {
        public Province()
        {
            RegisteredUserAddresses = new HashSet<RegisteredUserAddress>();
        }

        public int ProvinceId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<RegisteredUserAddress> RegisteredUserAddresses { get; set; }
    }
}
