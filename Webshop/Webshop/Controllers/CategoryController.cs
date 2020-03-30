using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webshop.Context;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    public class CategoryController : Controller
    {
        private readonly WebAPIHandler webAPI;
        private readonly WebAPIToken webAPIToken;

        public CategoryController(WebAPIHandler webAPI, WebAPIToken webAPIToken)
        {
            this.webAPI = webAPI;
            this.webAPIToken = webAPIToken;
        }
        
        public async Task<IActionResult> Index()
        {
            //var catlist = (from category in context.Categories
            //               select category).ToList();

            var categories = await webAPI.GetAllAsync<Category>("https://localhost:44305/api/categories");
            return View(categories);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditCategory(int? id)
        {
            EditCategoryModel editCategoryModel = new EditCategoryModel();
            if (id != null)
            {
                var category = await webAPI.GetOneAsync<Category>("https://localhost:44305/api/categories/" + id);

                editCategoryModel.Id = category.Id;
                editCategoryModel.Name = category.Name;
            }

            editCategoryModel.categoryCollection = await webAPI.GetAllAsync<Category>("https://localhost:44305/api/categories");

            return View(editCategoryModel);
        }


        [HttpPost]
        public async Task<IActionResult> EditCategory([Bind]EditCategoryModel model)
        {
            model.categoryCollection = await webAPI.GetAllAsync<Category>("https://localhost:44305/api/categories");// context.Categories.ToList();

            if (ModelState.IsValid)
            {
                // If category contains an Id, update it, else create new category!
                if (model.Id > 0)
                {
                    var result = model.categoryCollection.Where(x => x.Id == model.Id).FirstOrDefault();
                    result.Name = model.Name;

                    var token = await webAPIToken.New();
                    var response = await webAPI.UpdateAsync(result, "https://localhost:44305/api/categories/" + result.Id, token);

                    TempData["CategoryUpdate"] = "Kategorin har uppdaterats!";
                }
                else
                {
                    // Does category already exist?
                    if (model.categoryCollection.Any(x => x.Name == model.Name))
                    {
                        ModelState.AddModelError("Name", "Kategorin finns redan registrerad!");
                        return View("EditCategory", model);
                    }

                    // Create new category
                    var category = new Category() { Name = model.Name };

                    // Post to API
                    var token = await webAPIToken.New();
                    var response = await webAPI.PostAsync<Category>(category, "https://localhost:44305/api/categories/", token);

                    TempData["NewCategory"] = "Ny kategori har skapats!";
                }

                return RedirectToAction("EditCategory", "Category", new { id = "" });

            }
            else
            {
                return View("EditCategory", model);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await webAPI.GetOneAsync<Category>("https://localhost:44305/api/categories/" + id);
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await webAPI.GetOneAsync<Category>("https://localhost:44305/api/categories/" + id);

            var token = await webAPIToken.New();
            var response = await webAPI.DeleteAsync("https://localhost:44305/api/categories/" + id, token);

            if (response)
                TempData["DeletedCategory"] = "Kategori " + category.Name + " och alla produkter har readerats";
            else
                TempData["DeletedCategoryFail"] = "Kunde inte radera " + category.Name + ". Kontakta support om problemet kvarstår!";

            return RedirectToAction("EditCategory", "Category", new { id = "" });
        }
    }
}