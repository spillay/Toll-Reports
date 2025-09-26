using System.Transactions;

namespace Toll.Reporting.Api.Repositories
{
    public interface IComprehensiveRepository
    {
        /// <summary>
        /// Returns a list of transactions used to build the comprehensive report.
        /// </summary>
        /// <returns>A collection of transactions.</returns>
        Task<IEnumerable<Transaction>> GetComprehensiveReportAsync();
    }
}
