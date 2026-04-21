using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MIS.Web.Models.AccountHistory
{
    public class AccountHistoryModel
    {
        [JsonProperty("laneName")]
        public string? LaneName { get; set; }

        [JsonProperty("transactionType")]
        public string? TransactionType { get; set; }

        [JsonProperty("transactionAmount")]
        public decimal TransactionAmount { get; set; }

        [JsonProperty("topUpAmount")]
        public decimal TopUpAmount { get; set; }

        [JsonProperty("userBalance")]
        public decimal UserBalance { get; set; }

        [JsonProperty("paymentMethod")]
        public string? PaymentMethod { get; set; }

        [JsonProperty("transactionDateTime")]
        public DateTime? TransactionDateTime { get; set; }

        [JsonProperty("registeredIdentifier")]
        public string? RegisteredIdentifier { get; set; }

        // JSON is "numberPlate" - name can stay, mapping is correct
        [JsonProperty("numberPlate")]
        public string? NumberPlateDetails { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }
    }
    public class AccountSearchItem
    {
        public string? AccountNumber { get; set; }
        public string? Description { get; set; }
        public decimal Balance { get; set; }
    }
}