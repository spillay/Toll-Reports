using MIS.Web.Models.EndOfDay;

namespace MIS.Web.Services.Interfaces
{
    public interface IEndOfDayReportService
    {
        Task<EndOfDayReportViewModel?> GetEndOfDayAsync(DateTime startDate, DateTime endDate, int? shiftId = null);
    }
}