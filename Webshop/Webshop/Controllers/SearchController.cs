using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    public class SearchController : Controller
    {
        private readonly WebAPIHandler webAPI;
        public List<AllProductsViewModel> allproducts;

        public SearchController( WebAPIHandler webAPI)
        {
            this.webAPI = webAPI;
        }

        public async Task<ActionResult<IEnumerable<AllProductsViewModel>>> Index(string searchtext)
        {
            List<AllProductsViewModel> products = new List<AllProductsViewModel>();

            // Anything to search for
            if (searchtext != null)
            {
                // Send a request to API
                products = await webAPI.GetAllAsync<AllProductsViewModel>(ApiURL.SEARCH + searchtext.ToLower());
                return View(products);
            }

            return RedirectToAction("AllProducts", "Product");
        }
    }
}