namespace Toll.Reporting.Api.DTOs
{
    public class ComprehensiveDto
    {
        public string? LaneName { get; set; }
        public string? TransactionType { get; set; }
        public string? DiscountType { get; set; } // ✅ Description only
        public DateTime TransactionDateTime { get; set; }
        public string? Shift { get; set; }
        public string? ManualTollClass { get; set; }
        public int? TariffPlanId { get; set; }
        public double? AmountInclusive { get; set; }
        public string? MethodOfPayment { get; set; }
        public string TollOperatorID { get; internal set; }
    }
}
