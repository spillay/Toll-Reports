using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toll.Reporting.Api.DTOs;

namespace Toll.Reporting.Api.Repositories.Interfaces
{
    public interface IAccountUsageDetailsRepository
    {
        Task<List<AccountSearchResultDto>> SearchAccountsAsync(string q, int take = 20);

        Task<AccountUsageDetailsResponseDto> GetAccountUsageDetailsAsync(
            string accountNumber,
            DateTime startDate,
            DateTime endDate);
    }
}