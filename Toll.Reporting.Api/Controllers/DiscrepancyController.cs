using Microsoft.AspNetCore.Mvc;

namespace Toll.Reporting.Api.Controllers
{
    public class DiscrepancyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
