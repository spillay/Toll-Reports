namespace MIS.Web.Models.Comprehensive
{
    public class ComprehensiveReportViewModel
    {
        internal readonly int RevenueI;

        public string? methodOfPayment { get; set; }
        public string? rowType { get; set; }
        public string operational_Shift { get; set; } = string.Empty;
        public string? toll_Operator_ID { get; set; }
        public string? lane_Name { get; set; }
        public decimal classI { get; set; }
        public decimal classII { get; set; }
        public decimal classIII { get; set; }
        public decimal classM { get; set; }
        public decimal total { get; set; }
        

        public DateTime StartDate { get; set; } = new DateTime(2025, 08, 19, 0, 0, 0, DateTimeKind.Utc);
        public DateTime EndDate { get; set; } = new DateTime(2025, 08, 22, 0, 0, 0, DateTimeKind.Utc);
        public class GrandTotalRow
{
    public string RowType { get; set; } = string.Empty; // "Count", "Count %", "Revenue", "Revenue %"
    public decimal ClassI { get; set; }
    public decimal ClassII { get; set; }
    public decimal ClassIII { get; set; }
    public decimal ClassM { get; set; }
    public decimal Total { get; set; }
}

        

        /*
         * 
         *  {
    "methodOfPayment": "string",
    "rowType": "string",
    "classI": 0,
    "classII": 0,
    "classIII": 0,
    "classM": 0,
    "total": 0
  }
         * 
         * */
    }
}
