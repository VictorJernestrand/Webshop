using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.ViewComponents
{
    public class CategoryMenuListViewComponent : ViewComponent
    {
        private readonly WebAPIHandler webAPI;

        public CategoryMenuListViewComponent(WebAPIHandler webAPI)
        {
            this.webAPI = webAPI;
        }

        public async Task<IViewComponentResult> InvokeAsync(
        int maxPriority, bool isDone)
        {
            var items = await GetAllCategoriesFromWebAPIAsync();
            return View(items);
        }


        //private async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        //    => await _context.Categories.ToListAsync();


        // Get all categories from the WebAPI
        private async Task<IEnumerable<Category>> GetAllCategoriesFromWebAPIAsync()
        {
            return await webAPI.GetAllAsync<Category>("https://localhost:44305/api/categories");
        }

    }
}
