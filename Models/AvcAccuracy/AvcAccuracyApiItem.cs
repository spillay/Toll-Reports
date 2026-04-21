namespace MIS.Web.Models.AvcAccuracy
{
    public class AvcAccuracyApiItem
    {
        public int LaneId { get; set; }
        public string LaneName { get; set; } = string.Empty;

        public int TollClassId { get; set; }
        public string ClassDescription { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        public decimal ActualCount { get; set; }
        public decimal AdjustedCount { get; set; }

        public decimal ActualPercentage { get; set; }
        public decimal AdjustedPercentage { get; set; }
    }
}