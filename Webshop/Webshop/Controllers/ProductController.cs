using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Webshop.Context;

namespace Webshop.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            using (var db=new WebshopContext())
            {
                var query = (from product in db.Products
                             select product).ToList();
                return View(query);
            }
                
        }
    }
}