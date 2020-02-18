using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Webshop.Context;
using Webshop.Models;

namespace Webshop.Controllers
{
    public class ProductController : Controller
    {
        private readonly WebshopContext context;
        public ProductController(WebshopContext context)
        {
            this.context = context;
        }
        public IActionResult Index(int catid)
        {
           
                var query = (from product in context.Products
                             where product.CategoryId==catid
                             select product).ToList();
                return View(query);
           
                
        }
        [HttpGet]
        public IActionResult ViewallCategory()
        {
            ProductCategoryViewModel VM = new ProductCategoryViewModel();
            var catlist = (from category in context.Categories
                           select category).ToList();
            VM.catlist = new SelectList(catlist, "Id", "Name");
            return View(VM);

        }

        [HttpPost]       
        public IActionResult ViewallCategory(IFormCollection form)
        {
            
            var selectedvalue = form["selectedCategory"];
            
           
            return RedirectToAction("Index","Product",new { catid = selectedvalue});
        }

    }
}