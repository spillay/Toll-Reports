using System;
using System.Globalization;

namespace MIS.Web.Models.VarientPerfomance
{
    public class VarientPerfomanceModel
    {
        public DateTime? shiftDate { get; set; }
        public string? shiftDescription { get; set; } = string.Empty;
        public string? tollOperator { get; set; }
        public double NominalTariff { get; set; }
        public double NettAmount { get; set; }
        public DateTime? StartDate { get; set; } = DateTime.MinValue;
        public DateTime? EndDate { get; set; } = DateTime.MinValue;

    
    }
}
