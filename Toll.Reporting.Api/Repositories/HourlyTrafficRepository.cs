using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.DTOs;
using TollReportingSystem.Data;

namespace Toll.Reporting.Api.Repositories
{
    public class HourlyTrafficRepository : IHourlyTrafficRepository
    {
        private readonly ApplicationDbContext _context;

        public HourlyTrafficRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<HourlyTrafficDto>> GetHourlyTrafficForSingleDayAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? classifications = null,
            List<int>? shifts = null,
            bool? operationalDay = null)
        {
            // Treat null as false
            bool isOperationalDay = operationalDay ?? false;

            DateTime start, end;

            if (isOperationalDay)
            {
                // Operational day: 05:30 → next day 05:29:59
                start = startDate.Date.AddHours(5).AddMinutes(30);
                end = endDate.Date.AddDays(1).AddHours(5).AddMinutes(29).AddSeconds(59);
            }
            else
            {
                // Calendar day: 00:00 → 23:59:59
                start = startDate.Date;
                end = endDate.Date.AddDays(1).AddSeconds(-1);
                shifts = null; // ignore shifts for calendar day
            }

            // Base query with join
            var transactionsQuery =
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

            // Apply classification filter (case-insensitive)
            if (classifications != null && classifications.Any())
            {
                var loweredClasses = classifications.Select(c => c.ToLower()).ToList();
                transactionsQuery = transactionsQuery
                    .Where(x => loweredClasses.Contains(x.ClassDescription.ToLower()));
            }

            // Materialize to memory
            var transactions = await transactionsQuery.ToListAsync();

            // Apply shift filtering only if operational day
            if (isOperationalDay && shifts != null && shifts.Any())
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

            // Group by hour + classification
            var result = transactions
                .GroupBy(x => new
                {
                    x.TransactionDateTime.Year,
                    x.TransactionDateTime.Month,
                    x.TransactionDateTime.Day,
                    x.TransactionDateTime.Hour,
                    x.ClassDescription
                })
                .Select(g => new HourlyTrafficDto
                {
                    StartDate = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day, g.Key.Hour, 0, 0),
                    EndDate = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day, g.Key.Hour, 59, 59),
                    Classification = g.Key.ClassDescription,
                    Count = g.Count()
                })
                .OrderBy(x => x.StartDate)
                .ToList();

            return result;
        }
    }
}
