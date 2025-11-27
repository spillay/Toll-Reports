using MIS.Web.Models.EndOfDay;

namespace MIS.Web.Services.Interfaces
{
    public interface IEndOfDayReportService
    {
        Task<List<EndOfDayRowModel>> GetEndOfDayAsync(DateTime reportDate);
    }
}
