namespace Toll.Reporting.Api.DTOs
{
    /// <summary>
    /// DTO for Transaction details report, shaped like the SQL query.
    /// </summary>
    public class TransactionDetailsDto
    {
        public int Lane_Nr { get; set; }
        public string Trx_Sequence_Nr { get; set; }
        public string Trx_Date { get; set; }
        public string Trx_Time { get; set; }
        public string Operational_Shift { get; set; }
        public string Toll_Operator_ID { get; set; }
        public string Lane_Name { get; set; }
        public string Method_of_Payment { get; set; }
        public string Toll_Collector_Class { get; set; }
        public string AVC_Class { get; set; }
        public string Final_Class { get; set; }
        public decimal? Tariff { get; set; }
        public string Tac_Card_Number { get; set; }
    }
}
