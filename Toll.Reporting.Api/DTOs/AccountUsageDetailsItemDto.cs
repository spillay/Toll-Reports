using System;

namespace Toll.Reporting.Api.DTOs
{
    public class AccountUsageDetailsItemDto
    {
        // ACCOUNT INFO
        public string AccountNumber { get; set; }
        public string UserName { get; set; }
        public string VehicleRegNumber { get; set; }
        public string Status { get; set; }

        // BALANCES
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }

        // TRANSACTION DETAILS
        public string TransactionType { get; set; }
        public decimal NettAmount { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal NominalTariff { get; set; }
        public decimal VatAmount { get; set; }
        public DateTime? TransactionDateTime { get; set; }
        public string LaneName { get; set; }
        public string PaymentMethod { get; set; }   // Always string

        // TOP-UP DETAILS
        public decimal TopUpAmount { get; set; }
        public string TopUpMethod { get; set; }
        public DateTime? TopUpDateTime { get; set; }
    }
}
