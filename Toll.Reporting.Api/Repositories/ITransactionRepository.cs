using Toll.Reporting.Api.DTOs;

namespace Toll.Reporting.Api.Repositories
{
    public interface ITransactionRepository
    {

        /// <summary>
        /// Gets the detailed transaction report with optional filters.
        /// </summary>
        /// <param name="startDate">Start date for filtering transactions.</param>
        /// <param name="endDate">End date for filtering transactions.</param>
        /// <param name="operationalShift">Optional list of shift names (or "-- All --").</param>
        /// <param name="tollOperators">Optional list of toll operators (or "-- All --").</param>
        /// <param name="laneNames">Optional list of lane names (or "-- All --").</param>
        /// <param name="paymentMethods">Optional list of payment methods (or "-- All --").</param>
        /// <returns>A collection of TransactionDetailsDto with matching records.</returns>
        Task<IEnumerable<TransactionDetailsDto>> GetTransactionDetailsAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? operationalShift = null,
            List<string>? tollOperators = null,
            List<string>? laneNames = null,
            List<string>? paymentMethods = null);

        /// <summary>
        /// Gets distinct shift descriptions for filter dropdown.
        /// </summary>
        Task<IEnumerable<string>> GetShiftsAsync();

        /// <summary>
        /// Gets distinct usernames of system users for filter dropdown.
        /// </summary>
        Task<IEnumerable<string>> GetTollOperatorsAsync();

        /// <summary>
        /// Gets distinct lane names for filter dropdown.
        /// </summary>
        Task<IEnumerable<string>> GetLanesAsync();

        /// <summary>
        /// Gets distinct payment method descriptions for filter dropdown.
        /// </summary>
        Task<IEnumerable<string>> GetPaymentMethodsAsync();
    }
}

