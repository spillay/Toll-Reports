using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.DTOs
{
    public class AccountUsageDetailsReportDto
    {
        public SummaryDto Summary { get; set; } = new SummaryDto();
        public List<ItemDto> Details { get; set; } = new List<ItemDto>();

        // ----------------------------
        // Nested DTOs (single file)
        // ----------------------------

        public class SummaryDto
        {
            public int TotalAccounts { get; set; }

            public decimal TotalOpeningBalance { get; set; }
            public decimal TotalClosingBalance { get; set; }

            public decimal TotalTopUp { get; set; }
            public decimal TotalDeduct { get; set; }

            public decimal TotalNett { get; set; }
            public decimal TotalDiscount { get; set; }
            public decimal TotalNominal { get; set; }
            public decimal TotalVat { get; set; }

            public int TotalTransactions { get; set; }
            public int TotalTopUpCount { get; set; }

            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        public class ItemDto
        {
            // ACCOUNT INFO
            public string AccountNumber { get; set; } = "";
            public string UserName { get; set; } = "";
            public string VehicleRegNumber { get; set; } = "";
            public string Status { get; set; } = "";

            // BALANCES
            public decimal OpeningBalance { get; set; }
            public decimal ClosingBalance { get; set; }

            // TRANSACTION DETAILS
            public string TransactionType { get; set; } = "";
            public decimal NettAmount { get; set; }
            public decimal DiscountValue { get; set; }
            public decimal NominalTariff { get; set; }
            public decimal VatAmount { get; set; }
            public DateTime? TransactionDateTime { get; set; }
            public string LaneName { get; set; } = "";
            public string PaymentMethod { get; set; } = "";

            // TOP-UP DETAILS
            public decimal TopUpAmount { get; set; }
            public string TopUpMethod { get; set; } = "";
            public DateTime? TopUpDateTime { get; set; }
        }
    }
}