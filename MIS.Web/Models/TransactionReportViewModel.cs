namespace MIS.Web.Models
{
    public class TransactionReportViewModel
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
        public string? tac_Card_Number { get; set; }
        public DateTime StartDate { get; set; } = new DateTime(2025, 08, 19, 0, 0, 0, DateTimeKind.Utc);
        public DateTime EndDate { get; set; } = new DateTime(2025, 08, 22, 0, 0, 0, DateTimeKind.Utc);

        // Computed property → combine date + time into one DateTime
        public DateTime TransactionDateTime
        {
            get
            {
                if (DateTime.TryParseExact(trx_Date, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var datePart)
                 && DateTime.TryParseExact(trx_Time, "HH:mm:ss:fff", null, System.Globalization.DateTimeStyles.None, out var timePart))
                {
                    return datePart.Date.Add(timePart.TimeOfDay);
                }
                return DateTime.MinValue; // fallback
            }
        }
        //public DateTime startDate = DateTime.UtcNow;
        //public DateTime? TransactionDate { get; set;
        /*
         * 
         * {"lane_Nr":8,"trx_Sequence_Nr":"1343","trx_Date":"22/08/2025","trx_Time":"23:35:59:177","operational_Shift":"Shift Three",
         * "toll_Operator_ID":"-- None --","lane_Name":"06 WEST LEKKI TO IKOYI","method_of_Payment":"-- None --","toll_Collector_Class":"Class 4",
         * "avC_Class":"Class 4","final_Class":"Class 4","tariff":1000.00,"tac_Card_Number":null}
         * 
         * */
    }
}
