using Toll.Reporting.Api.DTOs;

namespace Toll.Reporting.Api.Repositories
{
    public interface IVarientPerformanceRepository
    {
        Task<PagedResult<VarientPerformanceDto>> GetVarientPerformanceAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            int page = 1,
            int pageSize = 10);

        Task<IEnumerable<string>> GetShiftsAsync();
        Task<IEnumerable<string>> GetTollOperatorsAsync();
    }
}
