using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.DTOs;
using TollReportingSystem.Data;

namespace Toll.Reporting.Api.Repositories
{
    public class MonthlyTrafficRepository : IMonthlyTrafficRepository
    {
        private readonly ApplicationDbContext _context;

        public MonthlyTrafficRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MonthlyTrafficDto>> GetMonthlyTrafficAsync(
            int? year = null,
            int? month = null,
            bool? operationalMonth = null,
            List<string>? classifications = null,
            List<int>? shifts = null)
        {
            var isOperational = operationalMonth ?? false;

            // ✅ Use local time because shifts are business-time rules
            var now = DateTime.Now;

            DateTime start;
            DateTime endExclusive;

            if (year.HasValue && month.HasValue)
            {
                start = new DateTime(year.Value, month.Value, 1);
                endExclusive = start.AddMonths(1);
            }
            else if (year.HasValue)
            {
                start = new DateTime(year.Value, 1, 1);
                endExclusive = start.AddYears(1);
            }
            else
            {
                start = new DateTime(now.Year, now.Month, 1);
                endExclusive = start.AddMonths(1);
            }

            // ✅ Operational month boundary (month starts at 05:30)
            if (isOperational)
            {
                start = start.AddHours(5).AddMinutes(30);
                endExclusive = endExclusive.AddHours(5).AddMinutes(30);
            }

            // ✅ Base query in SQL
            var query =
                from t in _context.Transactions.AsNoTracking()
                join tc in _context.TollClasses.AsNoTracking()
                    on t.ManualTollClassId equals tc.TollClassId into tcs
                from tc in tcs.DefaultIfEmpty()
                where t.TransactionDateTime >= start
                   && t.TransactionDateTime < endExclusive
                select new
                {
                    t.TransactionDateTime,
                    ClassDescription = (tc != null && tc.ClassDescription != null)
                        ? tc.ClassDescription
                        : "Unknown"
                };

            // ✅ Classification filter
            if (classifications != null && classifications.Any())
            {
                var normalized = classifications
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Select(c => c.Trim().ToLower())
                    .Distinct()
                    .ToList();

                query = query.Where(x => normalized.Contains(x.ClassDescription.ToLower()));
            }

            // ✅ Shift filter in SQL (only when operational)
            if (isOperational && shifts != null && shifts.Any())
            {
                query = query.Where(x =>
                    (shifts.Contains(1) && (
                        (x.TransactionDateTime.Hour > 5 || (x.TransactionDateTime.Hour == 5 && x.TransactionDateTime.Minute >= 30)) &&
                        (x.TransactionDateTime.Hour < 13 || (x.TransactionDateTime.Hour == 13 && x.TransactionDateTime.Minute <= 30))
                    ))
                    ||
                    (shifts.Contains(2) && (
                        (x.TransactionDateTime.Hour > 13 || (x.TransactionDateTime.Hour == 13 && x.TransactionDateTime.Minute >= 30)) &&
                        (x.TransactionDateTime.Hour < 21 || (x.TransactionDateTime.Hour == 21 && x.TransactionDateTime.Minute <= 30))
                    ))
                    ||
                    (shifts.Contains(3) && (
                        (x.TransactionDateTime.Hour > 21 || (x.TransactionDateTime.Hour == 21 && x.TransactionDateTime.Minute >= 30)) ||
                        (x.TransactionDateTime.Hour < 5 || (x.TransactionDateTime.Hour == 5 && x.TransactionDateTime.Minute < 30))
                    ))
                );
            }

            // ✅ Grouping in SQL
            var grouped = await query
                .GroupBy(x => new { x.TransactionDateTime.Year, x.TransactionDateTime.Month, x.ClassDescription })
                .Select(g => new MonthlyTrafficDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    StartDate = new DateTime(g.Key.Year, g.Key.Month, 1),
                    EndDate = new DateTime(g.Key.Year, g.Key.Month, 1).AddMonths(1).AddTicks(-1),
                    Classification = g.Key.ClassDescription,
                    Count = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ThenBy(x => x.Classification)
                .ToListAsync();

            return grouped;
        }

        public async Task<List<int>> GetAvailableYearsAsync()
        {
            return await _context.Transactions.AsNoTracking()
                .Select(t => t.TransactionDateTime.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToListAsync();
        }

        public async Task<List<int>> GetAvailableMonthsAsync(int year)
        {
            return await _context.Transactions.AsNoTracking()
                .Where(t => t.TransactionDateTime.Year == year)
                .Select(t => t.TransactionDateTime.Month)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
        }
        public async Task<List<string>> GetAvailableClassificationsAsync()
        {
            return await _context.Transactions.AsNoTracking()
                .Join(_context.TollClasses.AsNoTracking(),
                      t => t.ManualTollClassId,
                      tc => tc.TollClassId,
                      (t, tc) => tc.ClassDescription)
                .Where(x => x != null && x != "")
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();
        }
    }
}