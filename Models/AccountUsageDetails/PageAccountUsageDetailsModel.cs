using System.Collections.Generic;

namespace MIS.Web.Models.AccountUsageDetails
{
    public class PageAccountUsageDetailsModel
    {
        public List<AccountUsageDetailsRowModel> Items { get; set; } = new();
        public AccountUsageDetailsHeaderModel Header { get; set; } = new();
    }
}