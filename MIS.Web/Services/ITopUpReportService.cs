using MIS.Web.Models.TopUp;

namespace MIS.Web.Services
{
    public interface ITopUpReportService
    {
        // Razor page (paged results)
        Task<PageTopUpModel> GetTopUpAsync(
            DateTime startDate,
            DateTime endDate,
            string? shift = null,
            string? operatorId = null,
            string? lane = null,
            string? accountNumber = null,
            int page = 1,
            int pageSize = 30);

        // Export full dataset (no paging)
        Task<PageTopUpModel> GetTopUpFullAsync(
            DateTime startDate,
            DateTime endDate,
            string? shift = null,
            string? operatorId = null,
            string? lane = null,
            string? accountNumber = null);
    }
}
