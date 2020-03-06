using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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


        public CategoryController(WebshopContext context)
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
        //[HttpPost]
        //public IActionResult Index(IFormCollection form)
        //{
        //    var selectedvalue = form["ddcategory"];

        //    return RedirectToAction("Index", "Product", new { catid = selectedvalue });
            
        //}


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult EditCategory(int? id)
        {
            EditCategoryModel editCategoryModel = new EditCategoryModel();
            if (id != null)
            {
                var category = context.Categories.Where(x => x.Id == (int)id).FirstOrDefault();
                editCategoryModel.Id = category.Id;
                editCategoryModel.Name = category.Name;
            }

            editCategoryModel.categoryCollection = context.Categories.ToList();

            return View(editCategoryModel);
        }


        [HttpPost]
        public IActionResult EditCategory([Bind]EditCategoryModel model)
        {
            model.categoryCollection = context.Categories.ToList();

            if (ModelState.IsValid)
            {
                // If category contains an Id, update it, else create new category!
                if (model.Id > 0)
                {
                    var result = context.Categories.Find(model.Id);
                    result.Name = model.Name;

                    context.Update<Category>(result);   // Update category
                    model.Id = 0;
                    
                }
                else
                {
                    // Does category already exist?
                    if (context.Categories.Any(x => x.Name == model.Name))
                    {
                        ModelState.AddModelError("Name", "Kategori finns redan registrerad!");
                        return View("EditCategory", model);
                    }

                    // Create new category
                    var category = new Category()
                    {
                        Id = model.Id,
                        Name = model.Name
                    };

                    // Add new brand
                    context.Add<Category>(category);
                }

                // Save changes
                context.SaveChanges();
                return RedirectToAction("EditCategory", "Category", new { id = "" });
               // return RedirectToAction("EditCategory", "Category");
            }
            else
            {
                return View("EditCategory", model);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult DeleteCategory(int id)
        {
            var category = context.Categories.Find(id);
            return View(category);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = context.Categories.Find(id);

            context.Remove<Category>(category);
            context.SaveChanges();

            return RedirectToAction("EditCategory", "Category");
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