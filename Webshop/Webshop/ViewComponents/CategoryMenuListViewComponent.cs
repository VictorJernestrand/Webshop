using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Webshop.Context;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.ViewComponents
{
    public class CategoryMenuListViewComponent : ViewComponent
    {
        private readonly WebAPIHandler webAPI;
        private readonly WebshopContext _context;
        private readonly IHttpClientFactory _clientFactory;

        public CategoryMenuListViewComponent(WebshopContext context, IHttpClientFactory clientFactory)
        {
            this.webAPI = new WebAPIHandler(clientFactory);
            _context = context;
            _clientFactory = clientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(
        int maxPriority, bool isDone)
        {
            var items = await GetAllCategoriesAsync();
            return View(items);
        }


        private async Task<IEnumerable<Category>> GetAllCategoriesAsync()
            => await _context.Categories.ToListAsync();


        // Get all categories from the WebAPI
        //private async Task<IEnumerable<Category>> GetAllCategoriesFromWebAPIAsync()
        //{
        //    //WebAPIHandler<Category> webAPI = new WebAPIHandler<Category>(_clientFactory, "https://localhost:44305/api/categories");
        //    return await webAPI.GetAllFromWebAPIAsync<Category>("https://localhost:44305/api/categories");
        //}

    }
}
