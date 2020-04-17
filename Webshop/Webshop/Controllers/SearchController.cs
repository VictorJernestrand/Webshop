using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    public class SearchController : Controller
    {
        private readonly WebAPIHandler webAPI;
        private readonly WebAPIToken webAPIToken;
        public List<AllProductsViewModel> allproducts;

        public SearchController(WebAPIHandler webAPI, WebAPIToken webAPIToken)
        {
            this.webAPI = webAPI;
            this.webAPIToken = webAPIToken;
        }

        public async Task<ActionResult<IEnumerable<AllProductsViewModel>>> Index(string searchtext)
        {
            List<AllProductsViewModel> products = new List<AllProductsViewModel>();

            // Anything to search for
            if (searchtext != null)
            {
                // Admin search
                if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
                {
                    var token = await webAPIToken.New();
                    products = await webAPI.GetAllAsync<AllProductsViewModel>(ApiURL.SEARCH_ADMIN + searchtext.ToLower(), token);
                }
                else
                {
                    products = await webAPI.GetAllAsync<AllProductsViewModel>(ApiURL.SEARCH + searchtext.ToLower());
                }

                return View(products);
            }

            return RedirectToAction("AllProducts", "Product");
        }
    }
}