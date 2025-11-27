using System;
using System.Collections.Generic;

namespace MIS.Web.Models.EndOfDay
{
    public class PageEndOfDayModel
    {
        public DateTime? ReportDate { get; set; }
        public List<EndOfDayRowModel> Rows { get; set; } = new();
    }
}
