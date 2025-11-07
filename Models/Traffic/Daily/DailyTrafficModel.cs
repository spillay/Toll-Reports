using System;

namespace MIS.Web.Models.Traffic.Daily
{
    public class DailyTrafficModel
    {
        public DateTime Date { get; set; } 
        public string? Classification { get; set; }
        public int Count { get; set; }
    }
}
