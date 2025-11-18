using System;
using System.Globalization;

namespace MIS.Web.Models.Transaction
{
    public class TransactionModel
    {
        public string? lane_Nr { get; set; }
        public string? trx_Sequence_Nr { get; set; }      // keep string since backend returns string IDs
        public string? trx_Date { get; set; } = string.Empty;    // dd-MM-yyyy or dd/MM/yyyy
        public string? trx_Time { get; set; } = string.Empty;    // HH:mm:ss
        public string? operational_Shift { get; set; } = string.Empty;
        public string? toll_Operator_ID { get; set; } = string.Empty;
        public string? lane_Name { get; set; } = string.Empty;
        public string? method_of_Payment { get; set; } = string.Empty;
        public string? toll_Collector_Class { get; set; } = string.Empty;
        public string? avC_Class { get; set; } = string.Empty;
        public string? final_Class { get; set; } = string.Empty;
        public decimal? tariff { get; set; }
        public string? tac_Card_Number { get; set; }

        public DateTime? StartDate { get; set; } = DateTime.MinValue;
        public DateTime? EndDate { get; set; } = DateTime.MinValue;

        public double TotalTariff { get; set; }

        // ✅ Computed property to merge date + time back into one DateTime for sorting or display
        public DateTime TransactionDateTime
        {
            get
            {
                var culture = CultureInfo.InvariantCulture;

                // Handle both “dd/MM/yyyy” and “dd-MM-yyyy” safely
                string[] dateFormats = { "dd/MM/yyyy", "dd-MM-yyyy" };
                string[] timeFormats = { "HH:mm:ss", "HH:mm", "HH:mm:ss:fff" };

                if (DateTime.TryParseExact(trx_Date, dateFormats, culture, DateTimeStyles.None, out var datePart))
                {
                    if (!string.IsNullOrWhiteSpace(trx_Time) &&
                        DateTime.TryParseExact(trx_Time, timeFormats, culture, DateTimeStyles.None, out var timePart))
                    {
                        return datePart.Add(timePart.TimeOfDay);
                    }

                    return datePart;
                }

                return DateTime.MinValue;
            }
        }

        // ✅ Convenience properties for formatted output
        public string DisplayDate => TransactionDateTime == DateTime.MinValue ? "--" : TransactionDateTime.ToString("dd/MM/yyyy");
        public string DisplayTime => TransactionDateTime == DateTime.MinValue ? "--" : TransactionDateTime.ToString("HH:mm:ss");
    }
}
