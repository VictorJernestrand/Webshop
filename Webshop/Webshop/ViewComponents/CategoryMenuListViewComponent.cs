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
            var items = await GetCategoriesAsync();
            return View(items);
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }
    }
}
