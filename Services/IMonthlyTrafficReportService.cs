using MIS.Web.Models.Traffic.Monthly;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public interface IMonthlyTrafficReportService
    {
        // Main report
        Task<PageMonthlyTrafficModel> GetTrafficReportAsync(
            int? year = null,
            int? month = null,
            bool? operationalMonth = null,
            List<string>? classifications = null,
            List<int>? shifts = null
        );

        // Dropdown helpers
        Task<List<int>>? GetAvailableYearsAsync();
        Task<List<int>>? GetAvailableMonthsAsync(int year);
        Task<List<string>> GetAvailableClassificationsAsync();
    }
}
