using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class PaymentMethod
{
    public byte PaymentMethodId { get; set; }

    public string Description { get; set; }

    public virtual ICollection<RegisteredUserTopUp> RegisteredUserTopUps { get; set; } = new List<RegisteredUserTopUp>();
}
