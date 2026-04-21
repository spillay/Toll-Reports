using System;

namespace MIS.Web.Models.AccountHistory
{
    public class AccountHistoryInputModel
    {
        public string? AccountNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? Operational { get; set; }

        public string? AccountHolder { get; set; }
        public string? AccountStatus { get; set; }
        public string? AccountType { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public double AccountBalance { get; set; }

        public PageAccountHistoryModel? PageData { get; set; }
        public List<AccountHistoryModel> FullRecords { get; set; } = new();
    }
}
