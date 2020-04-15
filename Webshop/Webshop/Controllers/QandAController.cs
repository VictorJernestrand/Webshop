using Microsoft.AspNetCore.Mvc;

namespace Webshop.Controllers
{
    public class QandAController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}