// File: Toll.Reporting.Api.DTOs/HourlyTrafficDto.cs
using System;

namespace Toll.Reporting.Api.DTOs
{
    public class HourlyTrafficDto
    {
       
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Classification name (e.g. "Class 1")
        public string Classification { get; set; } = string.Empty;

        // Count for this hour + classification
        public int Count { get; set; }
    }
}
