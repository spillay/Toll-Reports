using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class PaymentIdentifier
    {
        public PaymentIdentifier()
        {
            RegisteredUserTopUps = new HashSet<RegisteredUserTopUp>();
        }

        public byte PaymentIdentifierId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<RegisteredUserTopUp> RegisteredUserTopUps { get; set; }
    }
}
