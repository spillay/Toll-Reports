using MIS.Web.Models.Traffic;

namespace MIS.Web.Services
{
    public interface ITrafficReportService
    {
        Task<PageTrafficModel> GetTrafficReportAsync
            (
            int pageNumber,
            int pageSize,
            DateTime startDate,
            DateTime endDate,
            List<string>? classification = null
            );
    }
}
