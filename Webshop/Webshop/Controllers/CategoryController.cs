using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly WebAPIHandler webAPI;
        private readonly WebAPIToken webAPIToken;

        public CategoryController(WebAPIHandler webAPI, WebAPIToken webAPIToken)
        {
            this.webAPI = webAPI;
            this.webAPIToken = webAPIToken;
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int? id)
        {
            Category editCategoryModel = new Category();

            if (id != null)
                editCategoryModel = await GetCategoryById((int)id);

            editCategoryModel.categoryCollection = await GetAllCategories();

            return View(editCategoryModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory([Bind]Category model)
        {
            model.categoryCollection = await GetAllCategories();

            if (ModelState.IsValid)
            {
                // If category contains an Id, update it, else create new category!
                if (model.Id > 0)
                {
                    var result = model.categoryCollection.Where(x => x.Id == model.Id).FirstOrDefault();
                    result.Name = model.Name;

                    var token = await webAPIToken.New();
                    var response = await webAPI.UpdateAsync(result, ApiURL.CATEGORIES + result.Id, token);

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
                    var response = await webAPI.PostAsync<Category>(category, ApiURL.CATEGORIES, token);

                    TempData["NewCategory"] = "Ny kategori har skapats!";
                }

                return RedirectToAction("EditCategory", "Category", new { id = "" });

            }
            else
            {
                return View("EditCategory", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await GetCategoryById(id);
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Get category data to display for Admin
            var category = await GetCategoryById(id);

            // Delete
            var token = await webAPIToken.New();
            var response = await webAPI.DeleteAsync(ApiURL.CATEGORIES + id, token);

            // Display information
            if (response)
                TempData["DeletedCategory"] = "Kategori " + category.Name + " och alla produkter har readerats";
            else
                TempData["DeletedCategoryFail"] = "Kunde inte radera " + category.Name + ". Kontakta support om problemet kvarstår!";

            return RedirectToAction("EditCategory", "Category", new { id = "" });
        }




        // Get Brand by Id
        private async Task<Category> GetCategoryById(int id)
            => await webAPI.GetOneAsync<Category>(ApiURL.CATEGORIES + id);

        // Get all brands
        private async Task<List<Category>> GetAllCategories()
            => await webAPI.GetAllAsync<Category>(ApiURL.CATEGORIES);
    }
}