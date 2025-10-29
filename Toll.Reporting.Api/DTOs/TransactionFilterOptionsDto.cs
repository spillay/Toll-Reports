namespace Toll.Reporting.Api.DTOs
{
    public class TransactionFilterOptionsDto
    {
        public List<string> Shifts { get; set; } = new();
        public List<string> TollOperators { get; set; } = new();
        public List<string> Lanes { get; set; } = new();
        public List<string> PaymentMethods { get; set; } = new();
    }
}
