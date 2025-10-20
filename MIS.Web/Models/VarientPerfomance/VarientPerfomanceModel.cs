using System;

namespace MIS.Web.Models.VarientPerfomance
{
    public class VarientPerfomanceModel
    {
        public DateTime? ShiftDate { get; set; }
        public string? ShiftDescription { get; set; } = string.Empty;
        public string? TollOperator { get; set; }
        public double NominalTariff { get; set; }
        public double NettAmount { get; set; }
        public double Difference => NominalTariff - NettAmount;
    }
}
