using System;

namespace MIS.Web.Models.AccountUsageSummary
{
    public class AccountUsageSummaryModel
    {
        public string? AccountNumber { get; set; }
        public string? AccountStatus { get; set; }

        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }

        public int LaneTransactionCount { get; set; }
        public decimal LaneTransactionValue { get; set; }

        public int LaneDiscountCount { get; set; }
        public decimal LaneDiscountValue { get; set; }

        public int FrequentUserDiscountCount { get; set; }
        public decimal FrequentUserDiscountValue { get; set; }

        public int HappyHourDiscountCount { get; set; }
        public decimal HappyHourDiscountValue { get; set; }

        public int JourneyDiscountCount { get; set; }
        public decimal JourneyDiscountValue { get; set; }

        public decimal ReceiptTopUp { get; set; }
        public decimal ReceiptDeposit { get; set; }

        public decimal PaymentFees { get; set; }
        public decimal PaymentRefunds { get; set; }

        public decimal RefundAccount { get; set; }
        public decimal RefundDeposit { get; set; }
    }
}
