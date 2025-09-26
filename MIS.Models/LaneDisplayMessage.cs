using System;
using System.Collections.Generic;

#nullable disable

namespace MIS.Models
{
    public partial class LaneDisplayMessage
    {
        public int LaneDisplayMessageId { get; set; }
        public string English { get; set; }
        public string ToDisplay { get; set; }
    }
}
