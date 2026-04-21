using Newtonsoft.Json;
using System;

namespace MIS.Web.Models.VarientPerfomance
{
    public class VarientPerfomanceModel
    {
        [JsonProperty("shiftDate")]
        public DateTime? ShiftDate { get; set; }

        [JsonProperty("shiftDescription")]
        public string? ShiftDescription { get; set; }

        [JsonProperty("tollOperator")]
        public string? TollOperator { get; set; }

        // Expected amount from API
        [JsonProperty("nominalTariff")]
        public double? NominalTariff { get; set; }

        // Actual declared amount from API
        [JsonProperty("actualAmount")]
        public double? NettAmount { get; set; }

        // Staff performance difference from API
        [JsonProperty("difference")]
        public double? Difference { get; set; }

        [JsonProperty("discrepancyDifference")]
        public double? DiscrepancyDifference { get; set; }

        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }
    }
}