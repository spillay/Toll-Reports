using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class Country
    {
        public Country()
        {
            RegisteredUserAddresses = new HashSet<RegisteredUserAddress>();
        }

        public int CountryId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<RegisteredUserAddress> RegisteredUserAddresses { get; set; }
    }
}
