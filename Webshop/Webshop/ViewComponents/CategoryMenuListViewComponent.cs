using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Context;
using Webshop.Models;

namespace Webshop.ViewComponents
{
    public class CategoryMenuListViewComponent : ViewComponent
    {
        private readonly WebshopContext _context;

        public CategoryMenuListViewComponent(WebshopContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(
        int maxPriority, bool isDone)
        {
            var items = await GetAllCategoriesAsync();
            return View(items);
        }


        private async Task<IEnumerable<Category>> GetAllCategoriesAsync()
            => await _context.Categories.ToListAsync();

    }
}
