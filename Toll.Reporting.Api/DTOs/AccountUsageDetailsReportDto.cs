using System.Collections.Generic;

namespace Toll.Reporting.Api.DTOs
{
    public class AccountUsageDetailsReportDto
    {
        public AccountUsageDetailsTotalsDto Summary { get; set; }
        public List<AccountUsageDetailsItemDto> Details { get; set; }

        public AccountUsageDetailsReportDto()
        {
            Summary = new AccountUsageDetailsTotalsDto();
            Details = new List<AccountUsageDetailsItemDto>();
        }
    }
}
