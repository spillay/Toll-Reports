using MIS.Web.Models.Transaction;

namespace MIS.Web.Models
{
    public class TransactionInputModel : PageTransactionModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? lane_Nr { get; set; }
        public string? TollOperatorID { get; set; }
        public string? Shift { get; set; }
        public string? PaymentMethod { get; set; }

        public List<string> PaymentMethods { get; set; } = new();
        public List<string> Shifts { get; set; } = new();
        public List<string> TollOperators { get; set; } = new();
        public List<string> Lanes { get; set; } = new();
        public double TotalTariff { get; set; }

    }
}
