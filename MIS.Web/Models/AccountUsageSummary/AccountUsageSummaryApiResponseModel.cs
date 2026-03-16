namespace MIS.Web.Models.AccountUsageSummary
{
    public class AccountUsageSummaryApiResponseModel
    {
        public AccountUsageSummarySummaryModel Summary { get; set; } = new();
        public AccountUsageSummaryApiPagedDataModel Data { get; set; } = new();
    }
}