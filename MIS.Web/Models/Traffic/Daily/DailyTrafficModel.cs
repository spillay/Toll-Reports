using System;

namespace MIS.Web.Models.Traffic.Daily
{
    public class DailyTrafficModel
    {
        public DateTime Date { get; set; } // renamed from Period to Date for clarity
        public string? Classification { get; set; }
        public int Count { get; set; }
    }
}
