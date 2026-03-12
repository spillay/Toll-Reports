public class ComprehensiveInputModel
{
    public DateTime StartDate { get; set; } = DateTime.UtcNow.AddDays(-7);
    public DateTime EndDate { get; set; } = DateTime.UtcNow;

    public List<byte> ShiftIds { get; set; } = new();
    public List<long> OperatorIds { get; set; } = new();
    public List<int> LaneIds { get; set; } = new();
    public List<byte> DiscountTypeIds { get; set; } = new();
    public List<byte> TollClassIds { get; set; } = new();

    public List<byte> PaymentMethodIds { get; set; } = new();

    public string GroupBy { get; set; } = "TransactionType";
    
}