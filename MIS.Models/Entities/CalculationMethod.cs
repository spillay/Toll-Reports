using System;
using System.Collections.Generic;

namespace MIS.Models.Entities;

public partial class CalculationMethod
{
    public byte CalculationMethodId { get; set; }

    public string Description { get; set; }

    public virtual ICollection<DiscountType> DiscountTypes { get; set; } = new List<DiscountType>();
}
