using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webshop.Context;
using Webshop.Models;

namespace Webshop.Controllers
{
    public class CategoryController : Controller
    {
        private IWebHostEnvironment environment;

        private DatabaseCRUD databaseCRUD;
        private readonly WebshopContext context;
        public CategoryController(WebshopContext context, IWebHostEnvironment env)
        {
            this.context = context;
            databaseCRUD = new DatabaseCRUD(context);
            this.environment = env;
        }

        public IActionResult Index()
        {
            var catlist = (from category in context.Categories
                           select category).ToList();


            return View(catlist);
        }
        [HttpPost]
        public IActionResult Index(IFormCollection form)
        {
            var selectedvalue = form["ddcategory"];

            return RedirectToAction("Index", "Product", new { catid = selectedvalue });

        }
             // Create New Category
        [Authorize(Roles = "Admin")]
        public IActionResult GetCategoryAddPage()
        {

            return View();

        }
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> GetCategoryAddPage([Bind]CreateCategoryViewModel createCategoryViewModel)
        //public IActionResult GetCategoryAddPage([Bind]CreateCategoryViewModel createCategoryViewModel)
        {
            try
            {
                Category TheNewCataegory = new Category()
                {
                    Name = createCategoryViewModel.name
                };


                await databaseCRUD.InsertAsync<Category>(TheNewCataegory);
                TempData["Succesmsgcategory"] = $"Great!! {TheNewCataegory.Name} skapad i databasen";
                return RedirectToAction("AllProducts", "Product");
            }
            catch
            {
                TempData["Database error"] = "Sorry!! Något gick fel när du lägger Data till databasen";
                return RedirectToAction("GetCategoryAddPage", "Category");
            }
        }
                                            

    }
}