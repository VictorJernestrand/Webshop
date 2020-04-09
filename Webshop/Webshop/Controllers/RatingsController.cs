using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    public class RatingsController : Controller
    {
        private readonly WebAPIHandler webAPI;
        private readonly WebAPIToken webAPIToken;

        public Rating rating = new Rating();

        public RatingsController(WebAPIHandler webAPI, WebAPIToken webAPIToken)
        {
            this.webAPI = webAPI;
            this.webAPIToken = webAPIToken;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(AllProductsViewModel model)
        {
            Rating rating = model.NewRating;

            // Register rating with current date
            rating.RateDate = DateTime.UtcNow;

            rating.UserEmail = User.Identity.Name;
            rating.UserId = User.UserId();

            // Request new token and store rating
            var token = await webAPIToken.New();
            var apiResponse = await webAPI.PostAsync<Rating>(rating, ApiURL.RATINGS_POST, token);

            // Was new rating saved successfully?
            if (apiResponse.Status.IsSuccessStatusCode)
                TempData["NewRatingSaved"] = true;
            else
                TempData["NewRatingFailed"] = true;

            return RedirectToAction("ProductDetail", "Product", new { id = rating.ProductId });

        }
    }
}