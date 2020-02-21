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
      

    }
}