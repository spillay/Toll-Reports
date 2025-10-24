namespace MIS.Web.Models.Comprehensive
{
    public class ComprehensiveModel
    {
        public string? MethodOfPayment { get; set; }
        public string? RowType { get; set; }
        public string OperationalShift { get; set; } = string.Empty;
        public string? TollOperatorID { get; set; }
        public string LaneName { get; set; } = string.Empty;
        public double AmountInclusive { get; set; }
        public long TransactionNumber { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public string? DiscountType { get; set; }
        public string? ManualTollClass { get; set; }
        public string? Shift { get; set; }
    }
}
