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

        // Expected amount from API field "nominalTariff"
        [JsonProperty("nominalTariff")]
        public double? NominalTariff { get; set; }

        // Actual amount from API field "actualAmount"
        [JsonProperty("actualAmount")]
        public double? NettAmount { get; set; }

        [JsonIgnore]
        public double? Difference
        {
            get
            {
                if (NominalTariff == null || NettAmount == null)
                    return null;
                return NominalTariff - NettAmount;
            }
        }

        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }
    }
}
