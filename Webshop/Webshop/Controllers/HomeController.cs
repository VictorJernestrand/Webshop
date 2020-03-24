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

namespace Webshop.Controllers
{
    public class HomeController : Controller
    {
        private readonly WebshopContext context;

        private UserManager<User> UserMgr { get; }

        public HomeController(WebshopContext context, UserManager<User> userManager)
        {
            this.context = context;
            UserMgr = userManager;
        }

        public IActionResult Index()
        {
            var products = context.Products.Include("Brand").Include("Category").Where(d => d.Discount > 0).ToList();
            List<AllProductsViewModel> allProducts = products.Select(x => new AllProductsViewModel(x)).OrderBy(p => p.Name).ToList();
            return View(allProducts);
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
