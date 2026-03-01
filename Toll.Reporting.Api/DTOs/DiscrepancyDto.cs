namespace Toll.Reporting.Api.DTOs
{
    public class DiscrepancyDto
    {
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

        public decimal? Tariff { get; set; }
        public decimal? Updated_Tariff { get; set; }

        public string TakenAction { get; set; } = string.Empty;

        // ==========================================================
        // ✅ FILTER OPTIONS (same style as TransactionDetailsDto)
        // ==========================================================
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<string>? Shifts { get; set; }
        public List<string>? TollOperators { get; set; }
        public List<string>? Lanes { get; set; }
        public List<string>? PaymentMethods { get; set; }
        public List<string>? TakenActions { get; set; }
    }
}