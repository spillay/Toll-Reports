using System.Collections.Generic;

namespace MIS.Web.Models.TopUp
{
    public class PageTopUpModel : PageModel
    {
        public List<TopUpModel>? items { get; set; } = new();
    }
}
