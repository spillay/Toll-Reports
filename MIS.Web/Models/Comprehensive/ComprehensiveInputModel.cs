using System;

namespace MIS.Web.Models.Comprehensive
{
    /// <summary>
    /// Input model for capturing filter values from the UI (query string or form).
    /// Keep simple — this maps directly from the filter form.
    /// </summary>
    public class ComprehensiveInputModel
    {
        public DateTime StartDate { get; set; } = DateTime.UtcNow.AddDays(-7);
        public DateTime EndDate { get; set; } = DateTime.UtcNow;
        public string? Shift { get; set; }
        public string? TransactionType { get; set; }
        public string? TollOperatorID { get; set; }
        public string? LaneName { get; set; }
        public string? MethodOfPayment { get; set; }
        public string? DiscountType { get; set; }
        public string? Classification { get; set; }
        public string GroupBy { get; set; } = "TransactionType";
    }
}
