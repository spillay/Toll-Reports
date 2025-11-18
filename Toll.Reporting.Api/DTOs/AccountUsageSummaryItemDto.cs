namespace Toll.Reporting.Api.DTOs
{
    public class AccountUsageSummaryItemDto
    {
        public string AccountNumber { get; set; }
        public string AccountStatus { get; set; }

        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }

        public int LaneTransactionCount { get; set; }
        public decimal LaneTransactionValue { get; set; }

        public int LaneDiscountCount { get; set; }
        public decimal LaneDiscountValue { get; set; }

        public decimal ReceiptTopUp { get; set; }
        public decimal ReceiptDeposit { get; set; }

        public decimal PaymentFees { get; set; }
        public decimal PaymentRefunds { get; set; }

        public decimal RefundAccount { get; set; }
        public decimal RefundDeposit { get; set; }
    }
}
