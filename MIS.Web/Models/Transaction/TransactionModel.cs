using System;
using System.Globalization;

namespace MIS.Web.Models.Transaction
{
    public class TransactionModel
    {
        public string? lane_Nr { get; set; }
        public int? trx_Sequence_Nr { get; set; }
        public string? trx_Date { get; set; } = string.Empty;    // dd/MM/yyyy expected
        public string? trx_Time { get; set; } = string.Empty;    // HH:mm:ss or HH:mm
        public string? operational_Shift { get; set; } = string.Empty;
        public string? toll_Operator_ID { get; set; }
        public string? lane_Name { get; set; }
        public string? method_of_Payment { get; set; }
        public string? toll_Collector_Class { get; set; }
        public string? avC_Class { get; set; }
        public string? final_Class { get; set; }
        public decimal? tariff { get; set; }
        public string? tac_Card_Number { get; set; }

        public DateTime? StartDate { get; set; } = DateTime.MinValue;
        public DateTime? EndDate { get; set; } = DateTime.MinValue;

        public DateTime? TransactionDateTime
        {
            get
            {
                var culture = CultureInfo.InvariantCulture;

                if (DateTime.TryParseExact(trx_Date, "dd/MM/yyyy", culture, DateTimeStyles.None, out var datePart))
                {
                    // Accept multiple time formats
                    string[] timeFormats = { "HH:mm:ss", "HH:mm", "HH:mm:ss:fff" };

                    if (DateTime.TryParseExact(trx_Time, timeFormats, culture, DateTimeStyles.None, out var timePart))
                    {
                        return datePart.Date.Add(timePart.TimeOfDay);
                    }

                    return datePart.Date;
                }

                return DateTime.MinValue;
            }
        }
    }
}
