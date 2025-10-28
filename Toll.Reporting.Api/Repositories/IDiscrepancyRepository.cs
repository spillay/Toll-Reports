
using Toll.Reporting.Api.DTOs;

namespace Toll.Reporting.Api.Repositories
{
    public interface IDiscrepancyRepository
    {
        Task<PagedResult<DiscrepancyDto>> GetDiscrepancyAsync(
           DateTime startDate,
           DateTime endDate,
           List<string>? operationalShift = null,
           List<string>? tollOperators = null,
           List<string>? laneNames = null,
           List<string>? paymentMethods = null,
           List<string>? takenAction = null,
           int page = 1,
           int pageSize = 50
        );
    }
}
