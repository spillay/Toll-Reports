using System;
using System.Collections.Generic;

namespace MIS.Web.Models.DailyCashup
{
    public class PageDailyCashupModel : PageModel
    {
      
        public List<DailyCashupModel>? Items { get; set; }
        public List<string>? Shifts { get; set; }
        public List<string>? TollOperators { get; set; }
        

        public  DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
