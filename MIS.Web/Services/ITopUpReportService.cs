using MIS.Web.Models;
using MIS.Web.Models.TopUp;

namespace MIS.Web.Services
{
    public interface ITopUpReportService
    {
        // Load checkbox options (global)
        Task<TopUpInputModel> GetTopUpFilterOptionsAsync();

        //  Razor page (paged results)
        Task<PageTopUpModel> GetTopUpAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? shifts = null,
            List<string>? operatorIds = null,
            List<string>? lanes = null,
            List<string>? paymentMethods = null,
            string? accountNumber = null,
            int page = 1,
            int pageSize = 30);

        // Export full dataset (no paging)
        Task<PageTopUpModel> GetTopUpFullAsync(
            DateTime startDate,
            DateTime endDate,
            List<string>? shifts = null,
            List<string>? operatorIds = null,
            List<string>? lanes = null,
            List<string>? paymentMethods = null,
            string? accountNumber = null);
    }
}