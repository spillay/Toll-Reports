namespace MIS.Web.Models.Comprehensive
{
    public class ComprehensiveReportViewModel
    {
        internal readonly int RevenueI;

        // Transaction info
        public string? MethodOfPayment { get; set; }
        public string? RowType { get; set; }
        public string OperationalShift { get; set; } = string.Empty;
        public string? TollOperatorID { get; set; }
        public string LaneName { get; set; } = string.Empty;
        public double AmountInclusive { get; set; }
        public long TransactionNumber { get; set; }
        public string TransactionType { get; set; } = string.Empty;

        // Added missing properties
        public string? ManualTollClass { get; set; }
        public string? Shift { get; set; }

        // Date filters (do not remove)
        public DateTime StartDate { get; set; } = new DateTime(2025, 08, 19, 0, 0, 0, DateTimeKind.Utc);
        public DateTime EndDate { get; set; } = new DateTime(2025, 08, 22, 0, 0, 0, DateTimeKind.Utc);

        // Vehicle classes (six total)
        public int Class0 { get; set; }
        public int Class1 { get; set; }
        public int Class2 { get; set; }
        public int Class3 { get; set; }
        public int Class4 { get; set; }
        public int Danfo { get; set; }
        public int MotorCycle { get; set; }

        // Optional: computed total of all classes
        public int TotalVehicles => Class0 + Class1 + Class2 + Class3 + Class4 + Danfo + MotorCycle;
    }
}
