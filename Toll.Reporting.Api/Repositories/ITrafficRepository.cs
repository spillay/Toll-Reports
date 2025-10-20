using Toll.Reporting.Api.DTOs;
using Toll.Reporting.Api.Enums;

namespace Toll.Reporting.Api.Repositories
{
    public interface ITrafficRepository
    {
        Task<PagedResult<TrafficDto>> GetTrafficAsync(
            DateTime startDate,
            DateTime endDate,
            ReportViewType viewType,
            List<string>? classification = null,
            int page = 1,
            int pageSize = 10
        );
    }
}
