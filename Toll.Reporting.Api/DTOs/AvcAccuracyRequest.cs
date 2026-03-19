using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.Models.AvcAccuracy
{
    public class AvcAccuracyRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<int>? ShiftIds { get; set; }
        public List<int>? LaneIds { get; set; }
        public List<int>? ClassIds { get; set; }
    }
}