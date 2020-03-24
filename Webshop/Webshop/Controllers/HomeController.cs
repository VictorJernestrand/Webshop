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
        private readonly ILogger<HomeController> _logger;

        private readonly DatabaseCRUD db;
        private readonly WebshopContext context;

        public new User User { get; set; }

        private UserManager<User> UserMgr { get; }

        public HomeController(WebshopContext context, UserManager<User> userManager, ILogger<HomeController> logger)
        {
            this.context = context;
            db = new DatabaseCRUD(context);
            UserMgr = userManager;
            _logger = logger;
        }

        private void GetLoggedInUserAsync()
        {
            
            //User = db.GetByIdAsync<User>(UserMgr.GetUserId(HttpContext.User));
        }

        /*public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }*/

        public IActionResult Index()
        {
            AllProductsViewModel allProducts = new AllProductsViewModel();
            List<AllProductsViewModel> allproductslist = new List<AllProductsViewModel>();


            //User = await UserMgr.GetUserAsync(HttpContext.User);
            //ViewData["UserName"] = User.FirstName;// HttpContext.Session.GetString(SessionCookies.USER_NAME);
            //var result = context.Categories.ToList();
            //return View(User);


            // var products = context.Products.Include(x => x.Brand).Include(x => x.Category).ToList();
            //  List<AllProductsViewModel> allProductswithDiscount = products.Select(x => new AllProductsViewModel(x)).OrderBy(p => p.Name).Where(d => d.Discount > 0).ToList();
          
            
            allProducts.productsDiscountlist = context.Products.Where(x => x.Discount > 0).Select(x => new Product()).ToList();

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
                                       productsDiscountlist=allProducts.productsDiscountlist
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
