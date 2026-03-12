using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class RegisteredUserFee
{
    public long RegisteredUserFeesId { get; set; }

    public string RegisteredUserFeesType { get; set; }

    public DateOnly RegisteredUserFeesEffectiveDate { get; set; }

    public decimal RegisteredUserFeesValue { get; set; }
}
