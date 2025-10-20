using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.DTOs;
using Toll.Reporting.Api.Enums;
using TollReportingSystem.Data;

namespace Toll.Reporting.Api.Repositories
{
    public class TrafficRepository : ITrafficRepository
    {
        private readonly ApplicationDbContext _context;

        public TrafficRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<TrafficDto>> GetTrafficAsync(
            DateTime startDate,
            DateTime endDate,
            ReportViewType viewType,
            List<string>? classifications = null,
            int page = 1,
            int pageSize = 10)
        {
            // Step 1: Base query
            var query = _context.Transactions
                .Where(t => t.TransactionDateTime >= startDate && t.TransactionDateTime < endDate.AddDays(1))
                .Include(t => t.ManualTollClass)
                .AsQueryable();

            // Step 2: Apply optional classification filter
            if (classifications != null && classifications.Any())
            {
                query = query.Where(t => classifications.Contains(t.ManualTollClass.ClassDescription));
            }

            // Step 3: Fetch raw transactions from DB
            var transactions = await query.ToListAsync();

            // Step 4: Normalize the period for each transaction
            var groupedResult = viewType switch
            {
                ReportViewType.Hourly => transactions
                    .Select(t => new
                    {
                        Period = new DateTime(
                            t.TransactionDateTime.Year,
                            t.TransactionDateTime.Month,
                            t.TransactionDateTime.Day,
                            t.TransactionDateTime.Hour, 0, 0),
                        Classification = t.ManualTollClass.ClassDescription
                    })
                    .GroupBy(x => new { x.Period, x.Classification })
                    .Select(g => new TrafficDto
                    {
                        Period = g.Key.Period,
                        Classification = g.Key.Classification,
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Period)
                    .ToList(),

                ReportViewType.Daily => transactions
                    .Select(t => new
                    {
                        Period = t.TransactionDateTime.Date, // truncated to day
                        Classification = t.ManualTollClass.ClassDescription
                    })
                    .GroupBy(x => new { x.Period, x.Classification })
                    .Select(g => new TrafficDto
                    {
                        Period = g.Key.Period,
                        Classification = g.Key.Classification,
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Period)
                    .ToList(),

                ReportViewType.Monthly => transactions
                    .Select(t => new
                    {
                        Period = new DateTime(t.TransactionDateTime.Year, t.TransactionDateTime.Month, 1),
                        Classification = t.ManualTollClass.ClassDescription
                    })
                    .GroupBy(x => new { x.Period, x.Classification })
                    .Select(g => new TrafficDto
                    {
                        Period = g.Key.Period,
                        Classification = g.Key.Classification,
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Period)
                    .ToList(),
                    _ => new List<TrafficDto>()
            };



            var pagedItems = groupedResult
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<TrafficDto>
            {
                Items = pagedItems,
                TotalCount = groupedResult.Count,
                Page = page,
                PageSize = pageSize
            };

        }
    }
}
