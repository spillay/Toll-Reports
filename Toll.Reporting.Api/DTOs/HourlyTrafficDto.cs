// File: Toll.Reporting.Api.DTOs/HourlyTrafficDto.cs
using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.DTOs
{
    public class HourlyTrafficDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Report row fields
        public string Classification { get; set; } = string.Empty;
        public int Count { get; set; }

        public List<string> Classifications { get; set; } = new();
        public List<int> Shifts { get; set; } = new();
    }
}