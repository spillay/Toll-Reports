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
            bool isOperational = operationalMonth ?? false;

            // Determine date range
            DateTime start;
            DateTime end;

            if (year.HasValue && month.HasValue)
            {
                start = new DateTime(year.Value, month.Value, 1);
                end = start.AddMonths(1).AddTicks(-1);
            }
            else if (year.HasValue) // Full year
            {
                start = new DateTime(year.Value, 1, 1);
                end = start.AddYears(1).AddTicks(-1);
            }
            else
            {
                // Default to current month
                var now = DateTime.UtcNow;
                start = new DateTime(now.Year, now.Month, 1);
                end = start.AddMonths(1).AddTicks(-1);
            }

            // 🔹 Base query: All transactions for the selected period
            var query =
                from t in _context.Transactions
                join tc in _context.TollClasses
                    on t.ManualTollClassId equals tc.TollClassId into tcs
                from tc in tcs.DefaultIfEmpty()
                where t.TransactionDateTime >= start && t.TransactionDateTime <= end
                select new
                {
                    t.TransactionDateTime,
                    ClassDescription = tc.ClassDescription ?? "Unknown"
                };

            // 🔹 Classification filter
            if (classifications != null && classifications.Any())
            {
                var loweredClasses = classifications.Select(c => c.ToLower()).ToList();
                query = query.Where(x => loweredClasses.Contains(x.ClassDescription.ToLower()));
            }

            var transactions = await query.ToListAsync();

            // 🔹 Apply operational month filtering using shifts
            if (isOperational && shifts != null && shifts.Any())
            {
                transactions = transactions.Where(t =>
                {
                    var hour = t.TransactionDateTime.Hour;
                    var minute = t.TransactionDateTime.Minute;
                    bool match = false;

                    foreach (var shift in shifts)
                    {
                        switch (shift)
                        {
                            case 1: // 05:30 - 13:30
                                match |= (hour > 5 || (hour == 5 && minute >= 30)) &&
                                         (hour < 13 || (hour == 13 && minute <= 30));
                                break;

                            case 2: // 13:30 - 21:30
                                match |= (hour > 13 || (hour == 13 && minute >= 30)) &&
                                         (hour < 21 || (hour == 21 && minute <= 30));
                                break;

                            case 3: // 21:30 - 05:29
                                match |= hour > 21 || (hour == 21 && minute >= 30) ||
                                         hour < 5 || (hour == 5 && minute < 30);
                                break;
                        }
                    }

                    return match;
                }).ToList();
            }

            // 🔹 Group by Year/Month/Class
            var grouped = transactions
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
                .ToList();

            return grouped;
        }

        // 🔹 Return years dynamically for dropdowns
        public async Task<List<int>> GetAvailableYearsAsync()
        {
            return await _context.Transactions
                .Select(t => t.TransactionDateTime.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToListAsync();
        }

        // 🔹 Return months for a given year dynamically
        public async Task<List<int>> GetAvailableMonthsAsync(int year)
        {
            return await _context.Transactions
                .Where(t => t.TransactionDateTime.Year == year)
                .Select(t => t.TransactionDateTime.Month)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
        }
    }
}
