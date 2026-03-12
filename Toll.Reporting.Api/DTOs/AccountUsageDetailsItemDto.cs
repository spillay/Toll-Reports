using System;

namespace Toll.Reporting.Api.DTOs
{
    public class AccountUsageDetailsItemDto
    {
        public string? EID_DeviceType { get; set; }
        public string? EID_DeviceNumber { get; set; }
        public string? VehicleRegNumber { get; set; }
        public string? VehicleClass { get; set; }

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