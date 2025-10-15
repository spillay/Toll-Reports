using System;

namespace Toll.Reporting.Api.DTOs
{
    public class VarientPerformanceDto
    {
        public DateTime? ShiftDate { get; set; }
        public string? ShiftDescription { get; set; }
        public string? TollOperator { get; set; } 
        public double? ActualAmount { get; set; }
        public double? NominalTariff { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
