using System;
using System.Collections.Generic;

namespace MIS.Web.Models.AvcAccuracy
{
    public class PageAvcAccuracyReportModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<int> SelectedShiftIds { get; set; } = new();
        public List<int> SelectedLaneIds { get; set; } = new();
        public List<int> SelectedClassIds { get; set; } = new();

        public List<AvcAccuracyFilterOptionModel> ShiftOptions { get; set; } = new();
        public List<AvcAccuracyFilterOptionModel> LaneOptions { get; set; } = new();
        public List<AvcAccuracyFilterOptionModel> ClassOptions { get; set; } = new();

        public List<AvcAccuracyLaneRowModel> Lanes { get; set; } = new();
        public AvcAccuracyTotalsModel GrandTotal { get; set; } = new();
    }
}