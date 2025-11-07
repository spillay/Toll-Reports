public class AccountHistoryDto
{
    public AccountHeaderDto? AccountHeader { get; set; }
    public List<AccountHistoryRecordDto> HistoryRecords { get; set; } = new();
}

public class AccountHeaderDto
{
    public string? AccountNumber { get; set; }
    public string? AccountHolder { get; set; }
    public string? AccountStatus { get; set; }
    public string? AccountType { get; set; }
    public string? MobileNumber { get; set; }
    public string? Email { get; set; }
    public decimal AccountBalance { get; set; }
}

public class AccountHistoryRecordDto
{
    public string? LaneName { get; set; }
    public string? TransactionType { get; set; }
    public decimal TransactionAmount { get; set; }
    public decimal TopUpAmount { get; set; }
    public decimal UserBalance { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime? TransactionDateTime { get; set; }
    public string? RegisteredIdentifier { get; set; }
    public string? NumberPlate { get; set; }
    public string? Description { get; set; }
}
