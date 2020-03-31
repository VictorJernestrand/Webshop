using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Webshop.Context;
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
            return View(allproductslist);
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
