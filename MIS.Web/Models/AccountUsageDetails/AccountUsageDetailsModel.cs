using System;

namespace MIS.Web.Models.AccountUsageDetails
{
    public class AccountUsageDetailsModel
    {
        public string? AccountNumber { get; set; }
        public string? AccountStatus { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }

        // ===============================
        // 🚘 Vehicle / EID Information
        // ===============================
        public string? EID_DeviceType { get; set; } = "N/A";
        public string? EID_DeviceNumber { get; set; } = "N/A";
        public string? VehicleRegNumber { get; set; } = "N/A";
        public string? VehicleClass { get; set; } = "N/A";

        // ===============================
        // 🚇 Lane Transactions
        // ===============================
        public int LaneTransactionCount { get; set; }
        public decimal LaneTransactionValue { get; set; }

        // ===============================
        // 🎟 Lane Discount
        // ===============================
        public int LaneDiscountCount { get; set; }
        public decimal LaneDiscountValue { get; set; }

        // ===============================
        // ⭐ Frequent User Discount
        // ===============================
        public int FrequentUserDiscountCount { get; set; }
        public decimal FrequentUserDiscountValue { get; set; }

        // ===============================
        // 🕒 Happy Hour Discount
        // ===============================
        public int HappyHourDiscountCount { get; set; }
        public decimal HappyHourDiscountValue { get; set; }

        // ===============================
        // 🔄 Return Journey Discount
        // ===============================
        public int ReturnJourneyDiscountCount { get; set; }
        public decimal ReturnJourneyDiscountValue { get; set; }

        // ===============================
        // 🧾 Receipts / Payments / Refunds
        // ===============================
        public decimal ReceiptTopUp { get; set; }
        public decimal ReceiptDeposit { get; set; }

        public decimal PaymentFees { get; set; }
        public decimal PaymentRefunds { get; set; }

        public decimal RefundAccount { get; set; }
        public decimal RefundDeposit { get; set; }
    }
}
