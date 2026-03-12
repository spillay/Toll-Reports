using System;

namespace MIS.Web.Models.AccountUsageDetails
{
    public class AccountUsageDetailsRowModel
    {
        public string? EID_DeviceType { get; set; } = "N/A";
        public string? EID_DeviceNumber { get; set; } = "N/A";
        public string? VehicleRegNumber { get; set; } = "N/A";
        public string? VehicleClass { get; set; } = "N/A";

        public decimal Balance { get; set; }

        public int LaneTransactionCount { get; set; }
        public decimal LaneTransactionValue { get; set; }

        public decimal ReceiptTopUp { get; set; }
        public decimal ReceiptDeposit { get; set; }

        public decimal PaymentFees { get; set; }
        public decimal PaymentRefunds { get; set; }

        public decimal RefundAccount { get; set; }
        public decimal RefundDeposit { get; set; }
    }
}