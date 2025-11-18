using MIS.Web.Models;
using System.Collections.Generic;

namespace MIS.Web.Models.AccountHistory
{
    public class PageAccountHistoryModel : PageModel
    {
        public List<AccountHistoryModel> Items { get; set; } 
    }
}
