using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
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

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Rating>> Delete(int id)
        {
            // Request a new token
            var token = await webAPIToken.New();

            // Get rating by id
            var rating = await webAPI.GetOneAsync<Rating>(ApiURL.RATING_BY_ID + id, token);

            // Get user by id
            rating.User = await webAPI.GetOneAsync<User>(ApiURL.USER_BY_ID + rating.UserId, token);

            // Get product by id
            var product = await webAPI.GetOneAsync<AllProductsViewModel>(ApiURL.PRODUCTS + rating.ProductId, token);

            rating.Product = new Product
            {
                Name = product.Name,
                Category = new Category { Name = product.Name },
                Brand = new Brand { Name = product.BrandName },
                Photo = product.Photo
            };

            return View(rating);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Rating rating)
        {
            var token = await webAPIToken.New();
            var isDeleted = await webAPI.DeleteAsync(ApiURL.RATING_BY_ID + rating.Id, token);

            if (isDeleted)
                TempData["RatingDeleted"] = $"{rating.UserEmail}'s kundomdömme har raderats!";

            return RedirectToAction("ProductDetail", "Product", new { id = rating.ProductId });
        }

    }
}