using Microsoft.EntityFrameworkCore;
using System.Transactions;
using TollReportingSystem.Data;

namespace Toll.Reporting.Api.Repositories
{
    public class ComprehensiveRepository : IComprehensiveRepository
    {
        private readonly ApplicationDbContext _context;

        public ComprehensiveRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //public async Task<IEnumerable<object>> GetComprehensiveReportAsync()
        //{
        //    // Replace with actual SQL or EF query
        //    return await Task.FromResult(
        //        _context.Transactions
        //                .GroupBy(t => t.SystemUserId)
        //                .Select(g => new { Category = g.Key, Total = g.Sum(x => x.VatAmout) })
        //                .ToList()
        //    );
        //}

        Task<IEnumerable<Transaction>> IComprehensiveRepository.GetComprehensiveReportAsync()
        {
            throw new NotImplementedException();
        }
    }
}