namespace MIS.Web.Models.Discrepancy
{
    public class DiscrepancyReportViewModel
    {
        public string? lane_Nr { get; set; }
        public int trx_Sequence_Nr { get; set; }
        public string trx_Date { get; set; } = string.Empty;
        public string trx_Time { get; set; } = string.Empty;
        public string operational_Shift { get; set; } = string.Empty;
        public string? toll_Operator_ID { get; set; }
        public string? lane_Name { get; set; }
        public string? method_of_Payment { get; set; }
        public string? toll_Collector_Class { get; set; }
        public string? avC_Class { get; set; }
        public string? final_Class { get; set; }
        public decimal tariff { get; set; }
        public decimal updated_tariff { get; set; }
        public string? takenAction { get; set; }

        // ADD THESE PROPERTIES
        public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-90);
        public DateTime EndDate { get; set; } = DateTime.Now;

        // Computed property → combine date + time into one DateTime
        public DateTime TransactionDateTime
        {
            get
            {
                if (DateTime.TryParseExact(trx_Date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var datePart)
                 && DateTime.TryParseExact(trx_Time, "HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out var timePart))
                {
                    return datePart.Date.Add(timePart.TimeOfDay);
                }
                return DateTime.MinValue; // fallback
            }
        }
    }
}
