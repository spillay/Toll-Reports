public class AccountHistoryDto
{
    public AccountHeaderDto? AccountHeader { get; set; }

    public List<AccountHistoryRecordDto> HistoryRecords { get; set; } = new();

    public decimal TotalTopUps
    {
        get
        {
            return HistoryRecords.Sum(x => x.TopUpAmount);
        }
    }

    public decimal TotalTransactions
    {
        get
        {
            return HistoryRecords.Sum(x => x.TransactionAmount);
        }
    }

    public decimal NetMovement
    {
        get
        {
            return TotalTopUps - TotalTransactions;
        }
    }
}

public class AccountHeaderDto
{
    public string AccountNumber { get; set; } = string.Empty;

    public string AccountHolder { get; set; } = string.Empty;

    public string AccountStatus { get; set; } = string.Empty;

    public string AccountType { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public decimal AccountBalance { get; set; }
}

public class AccountHistoryRecordDto
{
    public string LaneName { get; set; } = string.Empty;

    public string TransactionType { get; set; } = string.Empty;

    // Debit (Lane Transaction)
    public decimal TransactionAmount { get; set; }

    // Credit (TopUp)
    public decimal TopUpAmount { get; set; }

    public decimal UserBalance { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public DateTime TransactionDateTime { get; set; }

    public string RegisteredIdentifier { get; set; } = string.Empty;

    public string NumberPlate { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;


}
