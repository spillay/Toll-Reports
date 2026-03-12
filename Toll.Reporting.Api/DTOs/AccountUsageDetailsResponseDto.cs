using System.Collections.Generic;

namespace Toll.Reporting.Api.DTOs
{
    public class AccountUsageDetailsResponseDto
    {
        public AccountUsageDetailsHeaderDto Header { get; set; } = new();
        public List<AccountUsageDetailsItemDto> Details { get; set; } = new();
    }
}