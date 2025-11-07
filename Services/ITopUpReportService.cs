using MIS.Web.Models.TopUp;

namespace MIS.Web.Services
{
    public interface ITopUpReportService
    {
        Task<PageTopUpModel> GetTopUpAsync(
            DateTime startDate,
            DateTime endDate,
            string? operatorId = null,
            string? lane = null,
            string? shift = null,
            string? accountNumber = null,
            bool? operationalDate = null,
            int page = 1,
            int pageSize = 50);
    }
}
