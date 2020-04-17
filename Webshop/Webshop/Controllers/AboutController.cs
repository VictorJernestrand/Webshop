using Microsoft.AspNetCore.Mvc;

namespace Webshop.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}