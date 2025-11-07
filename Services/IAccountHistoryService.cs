using MIS.Web.Models.AccountHistory;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public interface IAccountHistoryService
    {
        Task<AccountHistoryInputModel> GetAccountHistoryAsync(string accountNumber);
        Task<List<string>> GetAccountsAsync(); // For dropdown
    }
}
