using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models;

public partial class CalculationMethod
{
    public byte CalculationMethodId { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<DiscountType> DiscountTypes { get; set; } = new List<DiscountType>();
}
