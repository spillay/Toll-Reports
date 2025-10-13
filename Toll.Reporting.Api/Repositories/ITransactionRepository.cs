using Toll.Reporting.Api.DTOs;

public interface ITransactionRepository
{
    Task<PagedResult<TransactionDetailsDto>> GetTransactionDetailsAsync(
        DateTime startDate,
        DateTime endDate,
        List<string>? operationalShift = null,
        List<string>? tollOperators = null,
        List<string>? laneNames = null,
        List<string>? paymentMethods = null,
        int page = 1,
        int pageSize = 10);

    Task<IEnumerable<string>> GetShiftsAsync();
    Task<IEnumerable<string>> GetTollOperatorsAsync();
    Task<IEnumerable<string>> GetLanesAsync();
    Task<IEnumerable<string>> GetPaymentMethodsAsync();
}
