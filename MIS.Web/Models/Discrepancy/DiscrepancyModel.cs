using System;

namespace MIS.Web.Models.Discrepancy
{
    public class DiscrepancyModel
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
        public decimal Tariff { get; set; }
        public decimal Updated_Tariff { get; set; }
        public string TakenAction { get; set; } = string.Empty;

        public decimal TariffDifference => Updated_Tariff - Tariff;

        public DateTime TransactionDateTime
        {
            get
            {
                if (DateTime.TryParseExact(Trx_Date, "dd/MM/yyyy", null,
                    System.Globalization.DateTimeStyles.None, out var datePart)
                    && DateTime.TryParseExact(Trx_Time, "HH:mm:ss", null,
                    System.Globalization.DateTimeStyles.None, out var timePart))
                {
                    return datePart.Add(timePart.TimeOfDay);
                }

                if (DateTime.TryParse($"{Trx_Date} {Trx_Time}", out var dt))
                    return dt;

                return DateTime.MinValue;
            }
        }
    }
}
