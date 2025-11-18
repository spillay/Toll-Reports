using System.Collections.Generic;

namespace Toll.Reporting.Api.DTOs
{
    public class AccountUsageSummaryReportDto
    {
        public AccountUsageSummaryTotalsDto Summary { get; set; }
        public List<AccountUsageSummaryItemDto> Items { get; set; }

        public AccountUsageSummaryReportDto()
        {
            Summary = new AccountUsageSummaryTotalsDto();
            Items = new List<AccountUsageSummaryItemDto>();
        }
    }
}
