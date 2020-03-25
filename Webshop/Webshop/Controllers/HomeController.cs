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

            List<AllProductsViewModel> allproductslist = new List<AllProductsViewModel>();            


            // var products = context.Products.Include(x => x.Brand).Include(x => x.Category).ToList();
            //  List<AllProductsViewModel> allProductswithDiscount = products.Select(x => new AllProductsViewModel(x)).OrderBy(p => p.Name).Where(d => d.Discount > 0).ToList();
          
            

            allproductslist = context.Products.Include(x => x.Brand).Include(x => x.Category)
                                  .Select(x => new AllProductsViewModel()
                                  {
                                        Id = x.Id,
                                        Name = x.Name,
                                        Price = x.Price,
                                        Discount = x.Discount,
                                        DiscountPrice = x.Price - (x.Price * (decimal)x.Discount),//product.DiscountPrice;
                                        Quantity = x.Quantity,
                                        CategoryId = x.CategoryId,
                                        BrandId = x.BrandId,
                                        Description = x.Description,
                                        Photo = x.Photo != null ? x.Photo : "",
                                        BrandName = x.Brand.Name,
                                        CategoryName = x.Category.Name,
                                        Category = x.Category,
                                        Brand = x.Brand,
                                        FullDescription = x.FullDescription,
                                        Specification = x.Specification,
                                      
                                  }
                                  ).ToList();

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
