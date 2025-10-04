namespace Toll.Reporting.Api.DTOs
{
    public class ComprehensiveDto
    {
        public string MethodOfPayment { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Classification { get; set; }

        public string? RowType { get; set; }

        public decimal ClassI { get; set; }

        public decimal ClassII { get; set; }

        public decimal ClassIII { get; set; }

        public decimal ClassM { get; set; }
        public decimal Revenue { get; set; }

        public Total? Totals{ get; set; }
        public GrandTotal? GrandTotals{ get; set; }
    }
    public class GrandTotal
    {
        public int Count { get; set; }
        public decimal CountPerc { get; set; }
        public int Revenue { get; set; }
        public decimal RevenuePerc { get; set; }
    }
    public class Total
    {
        public int Count { get; set; }
        public decimal CountPerc { get; set; }
        public int Revenue { get; set; }
        public decimal RevenuePerc { get; set; }
    }
}
