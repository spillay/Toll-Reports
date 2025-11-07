using Microsoft.AspNetCore.Mvc;

namespace MIS.Web.Controllers
{
    public class LandingPageController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Welcome to LCC";
            ViewData["PageTitle"] = "Welcome to LCC";
            return View();
        }
    }
}