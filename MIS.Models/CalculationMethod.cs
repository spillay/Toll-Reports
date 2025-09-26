using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class CalculationMethod
    {
        public CalculationMethod()
        {
            DiscountTypes = new HashSet<DiscountType>();
        }

        public byte CalculationMethodId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<DiscountType> DiscountTypes { get; set; }
    }
}
