using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        //[BindProperty]
        //public Category category { get; set; }
        public IActionResult Index()
        {

        
        return View(context.Categories);
    }
    }
}