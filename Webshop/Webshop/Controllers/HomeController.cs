using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    public class HomeController : Controller
    {
        private readonly WebAPIHandler webAPI;

        public HomeController(WebAPIHandler webAPI)
        {
            this.webAPI = webAPI;
        }

        public async Task<IActionResult> Index()
        {
            var allproductslist = await webAPI.GetAllAsync<AllProductsViewModel>(ApiURL.PRODUCTS);
            var news = await webAPI.GetAllAsync<News>(ApiURL.NEWS_TOP5);

            HomeModel home = new HomeModel()
            {
                AllProducts = allproductslist,
                News = news
            };

            return View(home);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
