public class AccountHistoryDto
{
    public AccountHeaderDto? AccountHeader { get; set; }

    // Main dataset
    public List<AccountHistoryRecordDto> HistoryRecords { get; set; } = new();

    // NEW: Useful for exports and UI totals
    public decimal TotalTopUps => HistoryRecords.Sum(x => x.TopUpAmount);

    public decimal TotalTransactions => HistoryRecords.Sum(x => x.TransactionAmount);

    // NEW: Combined total
    public decimal NetMovement => TotalTopUps - TotalTransactions;
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

    // Debit (Lane)
    public decimal TransactionAmount { get; set; }

    // Credit (Top-Up)
    public decimal TopUpAmount { get; set; }

    public decimal UserBalance { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime? TransactionDateTime { get; set; }

    public string? RegisteredIdentifier { get; set; }
    public string? NumberPlate { get; set; }

    // Free-text description
    public string? Description { get; set; }
}
