using Microsoft.AspNetCore.Mvc;

namespace Webshop.Controllers
{
    public class CookiePolicyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}