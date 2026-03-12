using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class RegisteredUserFee
{
    public long RegisteredUserFeesId { get; set; }

    public string RegisteredUserFeesType { get; set; } = null!;

    public DateOnly RegisteredUserFeesEffectiveDate { get; set; }

    public decimal RegisteredUserFeesValue { get; set; }
}
