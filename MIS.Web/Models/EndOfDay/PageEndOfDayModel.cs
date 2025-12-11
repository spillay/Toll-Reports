using System;
using MIS.Web.Models.EndOfDay;

namespace MIS.Web.Models.EndOfDay
{
    public class PageEndOfDayModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Holds the full report returned from API
        public EndOfDayReportViewModel? Report { get; set; }
    }
}
