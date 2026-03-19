namespace Toll.Reporting.Api.Models.AvcAccuracy
{
    public class AvcAccuracyBaseRow
    {
        public int LaneId { get; set; }
        public string LaneName { get; set; } = string.Empty;

        public int TollClassId { get; set; }
        public string ClassDescription { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        public int ActualCount { get; set; }
        public int AdjustedCount { get; set; }

        public decimal ActualPercentage { get; set; }
        public decimal AdjustedPercentage { get; set; }
    }
}