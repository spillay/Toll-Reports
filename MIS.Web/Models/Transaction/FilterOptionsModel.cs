namespace MIS.Web.Models.Transaction
{
    public class FilterOptionsModel
    {
        public List<string> PaymentMethods { get; set; } = new();
        public List<string> Shifts { get; set; } = new();
        public List<string> TollOperators { get; set; } = new();
        public List<string> Lanes { get; set; } = new();
    }
}
