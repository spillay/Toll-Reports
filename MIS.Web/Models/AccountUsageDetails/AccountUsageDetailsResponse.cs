using System.Collections.Generic;

namespace MIS.Web.Models.AccountUsageDetails
{
    public class AccountUsageDetailsResponse
    {
        public AccountUsageDetailsHeaderModel Header { get; set; } = new();
        public List<AccountUsageDetailsRowModel> Details { get; set; } = new();
    }
}