using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class TollClassSpecification
    {
        public byte TollClassSpecificationId { get; set; }
        public byte TollClassId { get; set; }
        public bool HeightBeforeFirstAxle { get; set; }
        public int MinAxleCount { get; set; }
        public int MaxAxleCount { get; set; }
        public int MinHeight1Count { get; set; }
        public int MaxHeight1Count { get; set; }
        public int MinHeight2Count1 { get; set; }
        public int MaxHeight2Count1 { get; set; }
    }
}
