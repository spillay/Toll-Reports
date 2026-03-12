using MIS.Web.Models.AccountHistory;

public interface IAccountHistoryService
{
    Task<AccountHistoryInputModel> GetAccountHistoryAsync(
        string? accountNumber,
        DateTime? startDate,
        DateTime? endDate,
        bool? operational);

    Task<List<AccountSearchItem>> SearchAccountsAsync(string q, int take = 20);
}