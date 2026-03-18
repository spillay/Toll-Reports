using Toll.Reporting.Api.DTOs.EndOfDay;

namespace Toll.Reporting.Api.Repositories
{
    public interface IEndOfDayReportRepository
    {
        Task<EndOfDayReportDto?> GetEndOfDayAsync(DateTime startDate, DateTime endDate, int? shiftId = null);
    }
}