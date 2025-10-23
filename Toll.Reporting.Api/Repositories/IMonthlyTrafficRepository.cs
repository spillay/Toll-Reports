using Toll.Reporting.Api.DTOs;

namespace Toll.Reporting.Api.Repositories
{
    public interface IMonthlyTrafficRepository
    {
        Task<List<MonthlyTrafficDto>> GetMonthlyTrafficAsync(
            int? year = null,
            int? month = null,
            bool? operationalMonth = null,
            List<string>? classifications = null,
            List<int>? shifts = null
        );

        Task<List<int>> GetAvailableYearsAsync();
        Task<List<int>> GetAvailableMonthsAsync(int year);
    }
}
