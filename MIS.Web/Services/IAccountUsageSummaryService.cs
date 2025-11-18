using MIS.Web.Models.AccountUsageSummary;
using System;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public interface IAccountUsageSummaryService
    {
        Task<PageAccountUsageSummaryModel> GetAccountUsageSummaryAsync(DateTime startDate, DateTime endDate);
    }
}
