using Microsoft.AspNetCore.Mvc;
using MIS.Models;
using MIS.Web.Models.Transaction;

namespace MIS.Web.Models
{
    public class TransactionInputModel : PageTransactionModel
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        // Bound filters

        [BindProperty(SupportsGet = true)] public string? lane_Nr { get; set; }
        [BindProperty(SupportsGet = true)] public string? TollOperatorID { get; set; }
        [BindProperty(SupportsGet = true)] public string? Shift { get; set; }
        [BindProperty(SupportsGet = true)] public string? PaymentMethod { get; set; }

    }
}
