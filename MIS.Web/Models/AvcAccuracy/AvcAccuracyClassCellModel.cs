namespace MIS.Web.Models.AvcAccuracy
{
    public class AvcAccuracyClassCellModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        public decimal ActualCount { get; set; }
        public decimal AdjustedCount { get; set; }

        public decimal ActualPercentage { get; set; }
        public decimal AdjustedPercentage { get; set; }
    }
}