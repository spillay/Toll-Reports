using Toll.Reporting.Api.DTOs;

namespace Toll.Reporting.Api.Repositories
{
    public interface ITransactionRepository
    {
        Task<PagedResult<TransactionDetailsDto>> GetTransactionDetailsAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            List<string>? laneNames = null,
            List<string>? paymentMethods = null,
            List<string>? tollCollectorClasses = null,
            int page = 1,
            int pageSize = 10);

        Task<TransactionDetailsDto> GetTransactionFilterOptionsAsync(DateTime startDate, DateTime endDate);
    }
}
