using System.Collections.Generic;

namespace MIS.Web.Models.AvcAccuracy
{
    public class AvcAccuracyTotalsModel
    {
        public List<AvcAccuracyClassCellModel> Classes { get; set; } = new();

        public decimal TotalActualCount { get; set; }
        public decimal TotalAdjustedCount { get; set; }

        public decimal TotalAccuracyActual { get; set; }
        public decimal TotalAccuracyAdjusted { get; set; }

        public decimal TotalTraffic { get; set; }
        public decimal TotalClassError { get; set; }
        public decimal TotalError { get; set; }
    }
}