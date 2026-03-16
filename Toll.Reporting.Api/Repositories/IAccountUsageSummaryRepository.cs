using System;
using System.Threading.Tasks;
using Toll.Reporting.Api.DTOs;

namespace Toll.Reporting.Api.Repositories.Interfaces
{
    public interface IAccountUsageSummaryRepository
    {
        Task<AccountUsageSummaryReportDto> GetSummaryAsync(
            DateTime startDate,
            DateTime endDate,
            string? accountNumber = null,
            int page = 1,
            int pageSize = 20);
    }
}