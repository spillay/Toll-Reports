using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class PaymentMethod
{
    public byte PaymentMethodId { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<RegisteredUserTopUp> RegisteredUserTopUps { get; set; } = new List<RegisteredUserTopUp>();
}
