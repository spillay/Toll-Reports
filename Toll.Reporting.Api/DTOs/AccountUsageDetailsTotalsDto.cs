using System;

namespace Toll.Reporting.Api.DTOs
{
    public class AccountUsageDetailsTotalsDto
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
}
    