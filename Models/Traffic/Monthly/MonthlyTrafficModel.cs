using System;

namespace MIS.Web.Models.Traffic.Monthly
{
    public class MonthlyTrafficModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string? Classification { get; set; }
        public int Count { get; set; }

        public DateTime MonthDate => new DateTime(Year, Month, 1);
    }
}
