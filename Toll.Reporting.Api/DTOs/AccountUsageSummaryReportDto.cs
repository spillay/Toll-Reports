namespace Toll.Reporting.Api.DTOs
{
    public class AccountUsageSummaryReportDto
    {
        public AccountUsageSummaryTotalsDto Summary { get; set; } = new();
        public global::PagedResult<AccountUsageSummaryItemDto> Data { get; set; } = new();
    }
}