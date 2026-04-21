using MIS.Web.Models.AccountUsageDetails;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public interface IAccountUsageDetailsService
    {
        Task<List<AccountSearchResultModel>> SearchAccountsAsync(string q, int take = 20);

        Task<PageAccountUsageDetailsModel> GetAccountUsageDetailsAsync(
            string accountNumber,
            DateTime startDate,
            DateTime endDate);
    }
}