using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.DTOs.EndOfDay;
using TollReportingSystem.Data;

namespace Toll.Reporting.Api.Repositories
{
    public class EndOfDayReportRepository : IEndOfDayReportRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EndOfDayReportRepository> _logger;

        public EndOfDayReportRepository(
            ApplicationDbContext context,
            ILogger<EndOfDayReportRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<EndOfDayReportDto?> GetEndOfDayAsync(
            DateTime startDate,
            DateTime endDate,
            int? shiftId = null)
        {
            try
            {
                var start = startDate.Date;
                var end = endDate.Date;

                var rows = (await _context.Database.GetDbConnection().QueryAsync<EndOfDayReportRowDto>(
                    "[star].[usp_GenerateEndOfDayReport]",
                    new
                    {
                        StartDate = start,
                        EndDate = end,
                        ShiftId = shiftId
                    },
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 300)).ToList();

                if (!rows.Any())
                {
                    return null;
                }

                return new EndOfDayReportDto
                {
                    StartDate = start,
                    EndDate = end,
                    MonthLabel = GetValueAfterLabel(rows, "Month:"),
                    IsOperationalDay = true,
                    OperationalDayLabel = GetOperationalDayLabel(rows),
                    ShiftName = GetValueAfterLabel(rows, "Operational Shift:"),
                    Rows = rows
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Repository failure in GetEndOfDayAsync. StartDate: {StartDate}, EndDate: {EndDate}, ShiftId: {ShiftId}",
                    startDate,
                    endDate,
                    shiftId);

                throw;
            }
        }

        private static string GetValueAfterLabel(IEnumerable<EndOfDayReportRowDto> rows, string label)
        {
            var row = rows.FirstOrDefault(x =>
                string.Equals((x.Col1 ?? string.Empty).Trim(), label, StringComparison.OrdinalIgnoreCase));

            return row?.Col2?.Trim() ?? string.Empty;
        }

        private static string GetOperationalDayLabel(IEnumerable<EndOfDayReportRowDto> rows)
        {
            var row = rows.FirstOrDefault(x =>
                string.Equals((x.Col1 ?? string.Empty).Trim(), "Operational Day:", StringComparison.OrdinalIgnoreCase));

            return row?.Col3?.Trim() ?? string.Empty;
        }
    }
}
