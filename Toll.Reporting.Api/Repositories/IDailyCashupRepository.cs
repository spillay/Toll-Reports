using Toll.Reporting.Api.DTOs;

namespace Toll.Reporting.Api.Repositories
{
    public interface IDailyCashupRepository
    {
        /// <summary>
        /// Fetch paginated daily cashup report results.
        /// </summary>
        Task<PagedResult<DailyCashupDto>> GetDailyCashupAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            int page = 1,
            int pageSize = 10);

        /// <summary>
        /// Get available filter options for dropdowns (shifts, operators).
        /// </summary>
        Task<DailyCashupFilterOptionsDto> GetDailyCashupFilterOptionsAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null);

        Task<IEnumerable<string>> GetShiftsAsync();
        Task<IEnumerable<string>> GetTollOperatorsAsync();
    }
}
