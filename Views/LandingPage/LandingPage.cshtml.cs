using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MIS.Web.Pages.LandingPage
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            ViewData["PageTitle"] = "Welcome to LCC";
        }
    }
}