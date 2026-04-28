namespace Toll.Reporting.Api.DTOs
{
    /// <summary>
    /// DTO for Transaction details report and filter options.
    /// </summary>
    public class TransactionDetailsDto
    {
        // -----------------------------
        // Transaction Data Fields
        // -----------------------------
        public int Lane_Nr { get; set; }
        public string Trx_Sequence_Nr { get; set; } = string.Empty;
        public string Trx_Date { get; set; } = string.Empty;
        public string Trx_Time { get; set; } = string.Empty;
        public string Operational_Shift { get; set; } = string.Empty;
        public string Toll_Operator_ID { get; set; } = string.Empty;
        public string Lane_Name { get; set; } = string.Empty;
        public string Method_of_Payment { get; set; } = string.Empty;
        public string Toll_Collector_Class { get; set; } = string.Empty;
        public string AVC_Class { get; set; } = string.Empty;
        public string Final_Class { get; set; } = string.Empty;
        public double? Tariff { get; set; }
        public string? Tac_Card_Number { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<string>? Shifts { get; set; }
        public List<string>? TollOperators { get; set; }
        public List<string>? Lanes { get; set; }
        public List<string>? PaymentMethods { get; set; }
        public List<string>? TollCollectorClasses { get; set; }
    }
}
