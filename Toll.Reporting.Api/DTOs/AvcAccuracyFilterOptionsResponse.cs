using System.Collections.Generic;

namespace Toll.Reporting.Api.Models.AvcAccuracy
{
    public class AvcAccuracyFilterOptionsResponse
    {
        public List<AvcAccuracyFilterOptionDto> Shifts { get; set; } = new();
        public List<AvcAccuracyFilterOptionDto> Lanes { get; set; } = new();
        public List<AvcAccuracyFilterOptionDto> Classes { get; set; } = new();
    }
}