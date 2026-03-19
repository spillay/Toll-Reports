using System;

namespace MIS.Web.Models.EndOfDay
{
    public class PageEndOfDayModel
    {
        public DateTime ReportDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? ShiftId { get; set; }

        public EndOfDayReportViewModel Report { get; set; } = new();
    }
}