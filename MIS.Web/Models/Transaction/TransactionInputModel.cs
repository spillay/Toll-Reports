using MIS.Web.Models.Transaction;
using System;
using System.Collections.Generic;

namespace MIS.Web.Models
{
    public class TransactionInputModel : PageTransactionModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<string> SelectedShifts { get; set; } = new();
        public List<string> SelectedTollOperators { get; set; } = new();
        public List<string> SelectedLanes { get; set; } = new();
        public List<string> SelectedPaymentMethods { get; set; } = new();
        public List<string> SelectedTollCollectorClasses { get; set; } = new();

        public List<string> PaymentMethods { get; set; } = new();
        public List<string> Shifts { get; set; } = new();
        public List<string> TollOperators { get; set; } = new();
        public List<string> Lanes { get; set; } = new();
        public List<string> TollCollectorClasses { get; set; } = new();

        // Totals & export
        public double TotalTariff { get; set; }
        public List<TransactionModel> ExportItems { get; set; } = new();
        public bool ExportAll { get; set; }
    }
}
