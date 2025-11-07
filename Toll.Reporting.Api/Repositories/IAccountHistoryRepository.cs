using System.Threading.Tasks;
using Toll.Reporting.Api.DTOs;

public interface IAccountHistoryRepository
{
   
    Task<AccountHistoryDto> GetAccountHistoryAsync(string accountNumber);
}
