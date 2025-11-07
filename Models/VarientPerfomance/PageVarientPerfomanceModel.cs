using System.Collections.Generic;

namespace MIS.Web.Models.VarientPerfomance
{
    public class PageVarientPerfomanceModel : PageModel
    {
        public List<VarientPerfomanceModel>? items { get; set; } // API returns 'items'
    }
}
