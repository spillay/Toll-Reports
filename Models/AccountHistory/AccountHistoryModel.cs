using System;

namespace MIS.Web.Models.AccountHistory
{
    public class AccountHistoryModel
    {
        public string? LaneName { get; set; }
        public string? TransactionType { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal TopUpAmount { get; set; }
        public decimal UserBalance { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? TransactionDateTime { get; set; }
        public string? RegisteredIdentifier { get; set; }
        public string? NumberPlate { get; set; }
    }
}
