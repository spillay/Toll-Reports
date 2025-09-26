using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class PaymentMethod
    {
        public PaymentMethod()
        {
            RegisteredUserTopUps = new HashSet<RegisteredUserTopUp>();
        }

        public byte PaymentMethodId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<RegisteredUserTopUp> RegisteredUserTopUps { get; set; }
    }
}
