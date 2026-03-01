using Toll.Reporting.Api.DTOs;

namespace Toll.Reporting.Api.Repositories
{
    public interface IDailyTrafficRepository
    {
  
        Task<List<DailyTrafficDto>> GetDailyTrafficAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? classifications = null,
            List<int>? shifts = null,
            bool? operationalDay = null
        );
        Task<List<string>> GetAllClassificationsAsync();
    }
}
