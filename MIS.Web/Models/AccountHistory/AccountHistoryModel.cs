using Newtonsoft.Json;
using System;

namespace MIS.Web.Models.AccountHistory
{
    public class AccountHistoryModel
    {
        [JsonProperty("LaneName")]
        public string? LaneName { get; set; }

        [JsonProperty("TransactionType")]
        public string? TransactionType { get; set; }

        [JsonProperty("TransactionAmount")]
        public decimal TransactionAmount { get; set; }

        [JsonProperty("TopUpAmount")]
        public decimal TopUpAmount { get; set; }

        [JsonProperty("UserBalance")]
        public decimal UserBalance { get; set; }

        [JsonProperty("PaymentMethod")]
        public string? PaymentMethod { get; set; }

        [JsonProperty("TransactionDateTime")]
        public DateTime? TransactionDateTime { get; set; }

        [JsonProperty("RegisteredIdentifier")]
        public string? RegisteredIdentifier { get; set; }

        [JsonProperty("NumberPlateDetails")]
        public string? NumberPlateDetails { get; set; }

        [JsonProperty("Description")]
        public string? Description { get; set; }
    }
}
