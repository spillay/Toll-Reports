using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class AddressType
    {
        public AddressType()
        {
            RegisteredUserAddresses = new HashSet<RegisteredUserAddress>();
        }

        public byte AddressTypeId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<RegisteredUserAddress> RegisteredUserAddresses { get; set; }
    }
}
