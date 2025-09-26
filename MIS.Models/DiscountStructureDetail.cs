using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class DiscountStructureDetail
    {
        public int DiscountStructureDetailId { get; set; }
        public int DiscountStructureId { get; set; }
        public byte TollClassId { get; set; }
        public double? Percentage { get; set; }
        public byte? MininumTrips { get; set; }
        public byte? MaxTrips { get; set; }
        public double? DiscountAmount { get; set; }
        public bool? CarryOverToNextMonth { get; set; }
        public double? CappAmount { get; set; }

        public virtual DiscountStructure DiscountStructure { get; set; }
        public virtual TollClass TollClass { get; set; }
    }
}
