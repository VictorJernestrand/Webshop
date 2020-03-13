using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webshop.Context;
using Webshop.Models;

namespace Webshop.Controllers
{
    public class SearchController : Controller
    {
        private readonly WebshopContext context;
        public List<AllProductsViewModel> allproducts;

        public SearchController(WebshopContext context)
        {
            this.context = context;
         
        }
        public IActionResult Index(string searchtext)
        {
            if (searchtext != null)
            {
                searchtext = searchtext.ToLower();

                var searchResult = context.Products.Include(x => x.Category)
                    .Include(x => x.Brand)
                    .Where(x => x.Name.ToLower().Contains(searchtext) ||
                        x.Brand.Name.ToLower().Contains(searchtext) ||
                        x.Category.Name.ToLower().Contains(searchtext) ||
                        x.Description.ToLower().Contains(searchtext) ||
                        x.FullDescription.ToLower().Contains(searchtext) ||
                        x.Specification.ToLower().Contains(searchtext)
                        )
                    .Select(x => new AllProductsViewModel(x))
                    .ToList();

                allproducts = searchResult;
                return View(allproducts);

            }
            return RedirectToAction("AllProducts", "Product");

        }
    }
}