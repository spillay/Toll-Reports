using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.DTOs;
using TollReportingSystem.Data;

namespace Toll.Reporting.Api.Repositories
{
    public class DailyTrafficRepository : IDailyTrafficRepository
    {
        private readonly ApplicationDbContext _context;

        public DailyTrafficRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DailyTrafficDto>> GetDailyTrafficAsync(
        DateTime startDate,
        DateTime endDate,
        List<string>? classifications = null,
        List<int>? shifts = null,
        bool? operationalDay = null)
            {
                DateTime start, end;

                if (operationalDay == true)
                {
                    start = startDate.Date.AddHours(5).AddMinutes(30);
                    end = endDate.Date.AddDays(1).AddHours(5).AddMinutes(29).AddSeconds(59);
                }
                else
                {
                    start = startDate.Date;
                    end = endDate.Date.AddDays(1).AddTicks(-1);
                }

                // Base query
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

                // Apply classification filter in SQL, case-insensitive, ignoring nulls
                if (classifications != null && classifications.Any())
                {
                    var loweredClasses = classifications.Select(c => c.ToLower()).ToList();
                    transactionsQuery = transactionsQuery
                        .Where(x => loweredClasses.Contains(x.ClassDescription.ToLower()));
                }

                // Materialize
                var transactions = await transactionsQuery.ToListAsync();

                //  Apply shift filter (only for operational day)
                if (operationalDay == true && shifts != null && shifts.Any())
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

                //  Group by day + class, with operational adjustment
                var result = transactions
                    .GroupBy(x => new
                    {
                        DateKey = (operationalDay == true && x.TransactionDateTime.Hour < 5)
                            ? x.TransactionDateTime.Date.AddDays(-1)
                            : x.TransactionDateTime.Date,
                        x.ClassDescription
                    })
                    .Select(g => new DailyTrafficDto
                    {
                        Date = g.Key.DateKey,
                        Classification = g.Key.ClassDescription ?? "Unknown",
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Date)
                    .ToList();

                return result;
            }

        public async Task<List<string>> GetAllClassificationsAsync()
        {
            return await _context.TollClasses
                .AsNoTracking()
                .Select(x => x.ClassDescription)
                .Where(x => x != null)
                .Select(x => x!.Trim())
                .Where(x => x != "")
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();
        }

    }
}
