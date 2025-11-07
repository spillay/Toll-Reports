using System.Collections.Generic;

namespace MIS.Web.Models.Discrepancy
{
    public class PageDiscrepancyModel : PageModel
    {
        public List<DiscrepancyModel> Items { get; set; } = new();
        public DiscrepancyInputModel Filters { get; set; } = new();
        public int TotalRecords => totalCount; 
    }
}
