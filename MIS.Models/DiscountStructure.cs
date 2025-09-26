using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class DiscountStructure
    {
        public DiscountStructure()
        {
            DiscountStructureDetails = new HashSet<DiscountStructureDetail>();
        }

        public int DiscountStructureId { get; set; }
        public byte DiscountTypeId { get; set; }
        public DateTime EffectiveDate { get; set; }

        public virtual DiscountType DiscountType { get; set; }
        public virtual ICollection<DiscountStructureDetail> DiscountStructureDetails { get; set; }
    }
}
