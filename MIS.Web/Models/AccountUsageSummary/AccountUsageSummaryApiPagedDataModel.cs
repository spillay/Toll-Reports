using System.Collections.Generic;

namespace MIS.Web.Models.AccountUsageSummary
{
    public class AccountUsageSummaryApiPagedDataModel
    {
        public List<AccountUsageSummaryModel> FullItems { get; set; } = new();
        public List<AccountUsageSummaryModel> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}