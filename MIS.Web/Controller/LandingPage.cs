using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MIS.Web.Controllers
{
    [Authorize]
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