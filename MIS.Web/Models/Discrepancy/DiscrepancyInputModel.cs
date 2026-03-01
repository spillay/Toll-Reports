using System;
using System.Collections.Generic;

namespace MIS.Web.Models.Discrepancy
{
    public class DiscrepancyInputModel
    {
        public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-90);
        public DateTime EndDate { get; set; } = DateTime.Now;

        public List<string> SelectedShifts { get; set; } = new();
        public List<string> SelectedTollOperators { get; set; } = new();
        public List<string> SelectedLanes { get; set; } = new();
        public List<string> SelectedPaymentMethods { get; set; } = new();
        public List<string> SelectedTakenActions { get; set; } = new();

        // paging
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;

        public List<string> Shifts { get; set; } = new();
        public List<string> TollOperators { get; set; } = new();
        public List<string> Lanes { get; set; } = new();
        public List<string> PaymentMethods { get; set; } = new();
        public List<string> TakenActions { get; set; } = new();
    }
}