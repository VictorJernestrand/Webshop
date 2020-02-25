using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webshop.Context;
using Webshop.Models;

namespace Webshop.Controllers
{
    public class CategoryController : Controller
    {
        private readonly WebshopContext context;
        public CategoryController(WebshopContext context)
        {
            this.context = context;
        }
        
        public IActionResult Index()
        {
            var catlist = (from category in context.Categories
                           select category).ToList();
            // var catlist = context.Categories.Select(x => x).ToList();
 
            return View(catlist);
        }
        [HttpPost]
        public IActionResult Index(IFormCollection form)
        {
            var selectedvalue = form["ddcategory"];

            return RedirectToAction("Index", "Product", new { catid = selectedvalue });
            
        }
    }
}