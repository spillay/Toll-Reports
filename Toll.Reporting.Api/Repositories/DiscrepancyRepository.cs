using TollReportingSystem.Data; // EF DbContext

namespace Toll.Reporting.Api.Repositories
{
    public class DiscrepancyRepository : IDiscrepancyRepository
    {
        private readonly ApplicationDbContext _context;

        public DiscrepancyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<object>> GetDiscrepancyReportAsync()
        {
            // Example: find mismatches between expected & actual
            return await Task.FromResult(
                _context.Transactions
                        .Where(t => t.ActualTollClass != t.ActualTollClass)
                        .Select(t => new { t.TransactionNumber, t.TransactionDateTime, t.TransactionTypeId })
                        .ToList()
            );
        }
    }
}