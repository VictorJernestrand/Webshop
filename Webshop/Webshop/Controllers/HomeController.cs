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
            //User = await UserMgr.GetUserAsync(HttpContext.User);
            //ViewData["UserName"] = User.FirstName;// HttpContext.Session.GetString(SessionCookies.USER_NAME);
            //var result = context.Categories.ToList();
            //return View(User);
            var products = context.Products.Include(x => x.Brand).Include(x => x.Category).ToList();
            List<AllProductsViewModel> allProducts = products.Select(x => new AllProductsViewModel(x)).OrderBy(p => p.Name).Where(d => d.Discount > 0).ToList();

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
