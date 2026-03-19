using System.Collections.Generic;

namespace MIS.Web.Models.AvcAccuracy
{
    public class AvcAccuracyFilterOptionsResponseModel
    {
        public List<AvcAccuracyFilterOptionModel> Shifts { get; set; } = new();
        public List<AvcAccuracyFilterOptionModel> Lanes { get; set; } = new();
        public List<AvcAccuracyFilterOptionModel> Classes { get; set; } = new();
    }
}