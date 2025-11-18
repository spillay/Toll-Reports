namespace Toll.Reporting.Api.DTOs
{
    public class AccountUsageSummaryTotalsDto
    {
        public int TotalAccounts { get; set; }

        public int Active { get; set; }
        public int Dormant { get; set; }
        public int Terminated { get; set; }

        public int TotalEIdDevices { get; set; }
        public int TotalEtcTags { get; set; }
        public int TotalSmartCards { get; set; }
        public int TotalVehicles { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
