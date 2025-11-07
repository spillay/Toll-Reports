using MIS.Web.Models.Traffic.Hourly;

namespace MIS.Web.Services
{
    public interface IHourlyTrafficReportService
    {
        Task<PageHourlyTrafficModel> GetTrafficReportAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? classification = null,
            List<int>? shifts = null,
            bool operationalDay = false
        );
    }
}
