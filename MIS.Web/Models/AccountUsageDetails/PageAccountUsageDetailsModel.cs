namespace MIS.Web.Models.AccountUsageDetails
{
    public class AccountUsageSummaryModel
    {
        public int TotalAccounts { get; set; }
        public int Active { get; set; }
        public int Dormant { get; set; }
        public int Terminated { get; set; }

        public int TotalEIdDevices { get; set; }
        public int TotalEtcTags { get; set; }
        public int TotalSmartCards { get; set; }

        public int TotalVehicles { get; set; }
    }

    public class PageAccountUsageDetailsModel : PageModel
    {
        public List<AccountUsageDetailsModel>? Items { get; set; }

        public AccountUsageSummaryModel Summary { get; set; } = new();
    }
}
