using System;

namespace Toll.Reporting.Api.DTOs
{
    public class AccountUsageDetailsHeaderDto
    {
        public string? AccountNumber { get; set; }
        public string? AccountStatus { get; set; }

        public decimal OpeningBalance { get; set; }
        public decimal TotalTopUps { get; set; }
        public decimal TotalTransactions { get; set; }
        public decimal TotalFees { get; set; }
        public decimal TotalDeposits { get; set; }
        public decimal TotalRefunds { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal DepositRefunded { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}