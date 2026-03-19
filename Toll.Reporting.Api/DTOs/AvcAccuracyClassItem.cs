using System.Collections.Generic;

namespace Toll.Reporting.Api.Models.AvcAccuracy
{
    public class AvcAccuracyClassItem
    {
        public int TollClassId { get; set; }
        public string ClassDescription { get; set; } = string.Empty;

        public int ActualCount { get; set; }
        public int AdjustedCount { get; set; }

        public decimal ActualPercentage { get; set; }
        public decimal AdjustedPercentage { get; set; }
    }

    public class AvcAccuracyLaneItem
    {
        public int LaneId { get; set; }
        public string LaneName { get; set; } = string.Empty;

        public List<AvcAccuracyClassItem> Classes { get; set; } = new();

        public int TotalActualCount { get; set; }
        public int TotalAdjustedCount { get; set; }
        public int TotalTraffic { get; set; }
        public int TotalClassErrorCount { get; set; }

        public decimal TotalAccuracyActual { get; set; }
        public decimal TotalAccuracyAdjusted { get; set; }
        public decimal TotalClassError { get; set; }
        public decimal TotalError { get; set; }
    }

    public class AvcAccuracyTotalClassItem
    {
        public int TollClassId { get; set; }
        public string ClassDescription { get; set; } = string.Empty;

        public int ActualCount { get; set; }
        public int AdjustedCount { get; set; }

        public decimal ActualPercentage { get; set; }
        public decimal AdjustedPercentage { get; set; }
    }

    public class AvcAccuracyResponse
    {
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;

        public List<int> SelectedShiftIds { get; set; } = new();
        public List<int> SelectedLaneIds { get; set; } = new();
        public List<int> SelectedClassIds { get; set; } = new();

        public List<AvcAccuracyLaneItem> Lanes { get; set; } = new();
        public List<AvcAccuracyTotalClassItem> TotalClasses { get; set; } = new();

        public int GrandTotalActualCount { get; set; }
        public int GrandTotalAdjustedCount { get; set; }
        public int GrandTotalTraffic { get; set; }
        public int GrandTotalClassErrorCount { get; set; }

        public decimal GrandTotalAccuracyActual { get; set; }
        public decimal GrandTotalAccuracyAdjusted { get; set; }
        public decimal GrandTotalClassError { get; set; }
        public decimal GrandTotalError { get; set; }
    }
}