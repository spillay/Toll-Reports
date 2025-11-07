namespace MIS.Web.Models.AccountHistory
{
    public class AccountHistoryInputModel
    {
        public string? AccountNumber { get; set; }

        // Account Header Info (from DB)
        public string? AccountHolder { get; set; }
        public string? AccountStatus { get; set; }
        public string? AccountType { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public decimal AccountBalance { get; set; }

        // Report Data
        public PageAccountHistoryModel? PageData { get; set; }
    }
}
