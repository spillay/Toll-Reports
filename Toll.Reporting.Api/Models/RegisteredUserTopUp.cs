using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class RegisteredUserTopUp
{
    public long RegisteredUserTopUpId { get; set; }

    public long RegisterUserId { get; set; }

    public long SystemUserId { get; set; }

    public DateTime RechargedOn { get; set; }

    public byte RechargeShift { get; set; }

    public string RechargeStation { get; set; } = null!;

    public string Description { get; set; } = null!;

    public byte PaymentMethodId { get; set; }

    public double Amount { get; set; }

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    public virtual RegisteredUser RegisterUser { get; set; } = null!;
}
