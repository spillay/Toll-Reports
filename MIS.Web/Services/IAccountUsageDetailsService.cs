using MIS.Web.Models.AccountUsageDetails;

namespace MIS.Web.Services
{
    public interface IAccountUsageDetailsService
    {
        // Return *combined* details+summary wrapped inside the page model
        Task<PageAccountUsageDetailsModel> GetAccountUsageDetailsAsync(DateTime start, DateTime end);

        // Provide summary separately
        Task<AccountUsageSummaryModel> GetSummaryAsync();
    }
}
