using MIS.Web.Models.VarientPerfomance;
using System;
using System.Collections.Generic;

namespace MIS.Web.Models
{
    public class VarientPerfomanceInputModel : PageVarientPerfomanceModel
    {
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-30);
        public DateTime EndDate { get; set; } = DateTime.Today;

        public List<string> OperationalShift { get; set; } = new();
        public List<string> TollOperators { get; set; } = new();

        // export dataset 
        public List<VarientPerfomanceModel> ExportItems { get; set; } = new();
    }
}