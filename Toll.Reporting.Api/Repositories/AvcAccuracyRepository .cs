using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.Models.AvcAccuracy;
using TollReportingSystem.Data;

namespace Toll.Reporting.Api.Repositories
{
    public class AvcAccuracyRepository : IAvcAccuracyRepository
    {
        private readonly ApplicationDbContext _context;

        public AvcAccuracyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AvcAccuracyBaseRow>> GetBaseDataAsync(AvcAccuracyRequest request)
        {
            var shiftIds = request.ShiftIds?.Distinct().ToList() ?? new List<int>();
            var laneIds = request.LaneIds?.Distinct().ToList() ?? new List<int>();
            var classIds = request.ClassIds?.Distinct().ToList() ?? new List<int>();

            var transactionsQuery = _context.Transactions
                .AsNoTracking()
                .Where(t => t.TransactionDateTime >= request.StartDate &&
                            t.TransactionDateTime <= request.EndDate);

            if (shiftIds.Any())
            {
                transactionsQuery = transactionsQuery.Where(t => shiftIds.Contains((int)t.ShiftId));
            }

            if (laneIds.Any())
            {
                transactionsQuery = transactionsQuery.Where(t => laneIds.Contains((int)t.LaneId));
            }

            var actualCounts = await transactionsQuery
                .Where(t => t.AutomaticTollClassId.HasValue)
                .Where(t => !classIds.Any() || classIds.Contains((int)t.AutomaticTollClassId.GetValueOrDefault()))
                .GroupBy(t => new
                {
                    LaneId = (int)t.LaneId,
                    TollClassId = (int)t.AutomaticTollClassId.GetValueOrDefault()
                })
                .Select(g => new
                {
                    g.Key.LaneId,
                    g.Key.TollClassId,
                    ActualCount = g.Count()
                })
                .ToListAsync();

            var adjustedCounts = await transactionsQuery
                .Where(t => t.ActualTollClassId.HasValue)
                .Where(t => !classIds.Any() || classIds.Contains((int)t.ActualTollClassId.GetValueOrDefault()))
                .GroupBy(t => new
                {
                    LaneId = (int)t.LaneId,
                    TollClassId = (int)t.ActualTollClassId.GetValueOrDefault()
                })
                .Select(g => new
                {
                    g.Key.LaneId,
                    g.Key.TollClassId,
                    AdjustedCount = g.Count()
                })
                .ToListAsync();

            var lanesQuery = _context.Lanes.AsNoTracking();

            if (laneIds.Any())
            {
                lanesQuery = lanesQuery.Where(l => laneIds.Contains((int)l.LaneId));
            }

            var lanes = await lanesQuery
                .Select(l => new
                {
                    LaneId = (int)l.LaneId,
                    l.LaneName
                })
                .ToListAsync();

            var tollClassesQuery = _context.TollClasses.AsNoTracking();

            if (classIds.Any())
            {
                tollClassesQuery = tollClassesQuery.Where(tc => classIds.Contains((int)tc.TollClassId));
            }

            var tollClasses = await tollClassesQuery
                .OrderBy(tc => tc.DisplayOrder)
                .Select(tc => new
                {
                    TollClassId = (int)tc.TollClassId,
                    tc.ClassDescription,
                    tc.DisplayOrder
                })
                .ToListAsync();

            var actualLookup = actualCounts.ToDictionary(
                x => (x.LaneId, x.TollClassId),
                x => x.ActualCount
            );

            var adjustedLookup = adjustedCounts.ToDictionary(
                x => (x.LaneId, x.TollClassId),
                x => x.AdjustedCount
            );

            var result = new List<AvcAccuracyBaseRow>();

            foreach (var lane in lanes)
            {
                foreach (var tollClass in tollClasses)
                {
                    actualLookup.TryGetValue((lane.LaneId, tollClass.TollClassId), out var actualCount);
                    adjustedLookup.TryGetValue((lane.LaneId, tollClass.TollClassId), out var adjustedCount);

                    var actualPercentage = adjustedCount == 0
                        ? 0m
                        : (actualCount * 100m) / adjustedCount;

                    var adjustedPercentage = actualCount == 0
                        ? 0m
                        : (adjustedCount * 100m) / actualCount;

                    result.Add(new AvcAccuracyBaseRow
                    {
                        LaneId = lane.LaneId,
                        LaneName = lane.LaneName,
                        TollClassId = tollClass.TollClassId,
                        ClassDescription = tollClass.ClassDescription,
                        DisplayOrder = tollClass.DisplayOrder,
                        ActualCount = actualCount,
                        AdjustedCount = adjustedCount,
                        ActualPercentage = decimal.Round(actualPercentage, 2),
                        AdjustedPercentage = decimal.Round(adjustedPercentage, 2)
                    });
                }
            }

            return result
                .OrderBy(x => x.LaneId)
                .ThenBy(x => x.DisplayOrder)
                .ToList();
        }

        public async Task<AvcAccuracyFilterOptionsResponse> GetFilterOptionsAsync()
        {
            var shifts = await _context.Shifts
                .AsNoTracking()
                .OrderBy(x => x.ShiftId)
                .Select(x => new AvcAccuracyFilterOptionDto
                {
                    Id = x.ShiftId,
                    Name = x.Description
                })
                .ToListAsync();

            var lanes = await _context.Lanes
                .AsNoTracking()
                .OrderBy(x => x.LaneId)
                .Select(x => new AvcAccuracyFilterOptionDto
                {
                    Id = x.LaneId,
                    Name = x.LaneName
                })
                .ToListAsync();

            var classes = await _context.TollClasses
                .AsNoTracking()
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new AvcAccuracyFilterOptionDto
                {
                    Id = x.TollClassId,
                    Name = x.ClassDescription
                })
                .ToListAsync();

            return new AvcAccuracyFilterOptionsResponse
            {
                Shifts = shifts,
                Lanes = lanes,
                Classes = classes
            };
        }
    }
}
