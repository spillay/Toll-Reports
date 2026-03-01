using System.Collections.Generic;

namespace MIS.Web.Models.TopUp
{
    public class PageTopUpModel : PageModel
    {
        public List<TopUpModel>? Items { get; set; } = new();

        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}