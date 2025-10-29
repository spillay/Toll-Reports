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

        public FilterOptionsModel FilterOptions { get; set; } = new();
    }
}
