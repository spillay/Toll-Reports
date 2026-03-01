using Toll.Reporting.Api.DTOs;

namespace Toll.Reporting.Api.Repositories
{
    public interface IDiscrepancyRepository
    {
        /// <summary>
        /// Retrieves discrepancy report data (paged by default).
        /// Use pageSize = int.MaxValue for exportAll behavior.
        /// </summary>
        Task<PagedResult<DiscrepancyDto>> GetDiscrepancyAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            List<string>? laneNames = null,
            List<string>? paymentMethods = null,
            List<string>? takenAction = null,
            int page = 1,
            int pageSize = 50);

        /// <summary>
        /// Retrieves filter checklist options for the discrepancy report.
        /// Returns ALL values from DB (used or not used).
        /// </summary>
        Task<DiscrepancyDto> GetDiscrepancyFilterOptionsAsync(
            DateTime startDate,
            DateTime endDate);
    }
}