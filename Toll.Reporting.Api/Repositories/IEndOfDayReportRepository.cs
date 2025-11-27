using Toll.Reporting.Api.DTOs;

namespace Toll.Reporting.Api.Repositories
{
    public interface IEndOfDayReportRepository
    {
        Task<List<EndOfDayRowDto>> GetEndOfDayReportAsync(DateTime reportDate);
    }

}
