using Toll.Reporting.Api.DTOs;

namespace Toll.Reporting.Api.Repositories.Interfaces
{
    public interface IAccountUsageSummaryRepository
    {
        Task<AccountUsageSummaryReportDto> GetSummaryAsync(DateTime startDate, DateTime endDate);
    }
}
