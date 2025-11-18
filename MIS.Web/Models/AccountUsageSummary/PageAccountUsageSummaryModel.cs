using System.Collections.Generic;

namespace MIS.Web.Models.AccountUsageSummary
{
    public class PageAccountUsageSummaryModel : PageModel
    {
        public List<AccountUsageSummaryModel>? Items { get; set; }
        public AccountUsageSummarySummaryModel? Summary { get; set; }
    }

    public class AccountUsageSummarySummaryModel
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
