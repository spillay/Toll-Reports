using System;
using System.Collections.Generic;

namespace MIS.Web.Models.Traffic.Daily
{
    public class DailyTrafficInputModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Classification { get; set; }
        public List<int> Shifts { get; set; } = new List<int>();
        public bool OperationalDay { get; set; } = false;
    }
}
