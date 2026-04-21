using Newtonsoft.Json;
using System;

namespace MIS.Web.Models.TopUp
{
    public class TopUpModel
    {
        [JsonProperty("topUpNumber")]
        public int TopUpNumber { get; set; }

        [JsonProperty("topUpDateTime")]
        public DateTime TopUpDateTime { get; set; }

        [JsonProperty("laneWorkstation")]
        public string? LaneWorkstation { get; set; }

        [JsonProperty("shift")]
        public string? Shift { get; set; }

        [JsonProperty("operator")]
        public string? Operator { get; set; }

        [JsonProperty("accountNumber")]
        public string? AccountNumber { get; set; }

        [JsonProperty("accountName")]
        public string? AccountName { get; set; }

        [JsonProperty("amountPaid")]
        public decimal AmountPaid { get; set; }

        [JsonProperty("methodOfPayment")]
        public string? MethodOfPayment { get; set; }
    }
}