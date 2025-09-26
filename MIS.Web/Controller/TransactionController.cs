using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace MIS.Web.Controller
{
    public class TransactionController : Microsoft.AspNetCore.Mvc.Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
