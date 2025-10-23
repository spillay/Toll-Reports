using System;

namespace Toll.Reporting.Api.DTOs
{
    public class MonthlyTrafficDto
    {
        public int Year { get; set; }
        public int Month { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Classification { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
