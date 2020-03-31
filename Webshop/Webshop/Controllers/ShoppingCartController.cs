using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webshop.Models;
using Webshop.Services;
using Webshop.Models.Data;
using System.Threading.Tasks;

namespace Webshop.Controllers
{
    public class ShoppingCartController : Controller
    {
        // SQL connection
        private readonly WebAPIHandler webAPI;

        public ShoppingCartController(WebAPIHandler webAPI)
        {
            this.webAPI = webAPI;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString(Common.CART_COOKIE_NAME) != null)
            {
                var cartId = HttpContext.Session.GetString(Common.CART_COOKIE_NAME);
                var result = await webAPI.GetAllAsync<ShoppingCartModel>("https://localhost:44305/api/carts/content/" + cartId);
                return View(result);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task AddToCart(int id)
        {
            // Generate a unique id
            Guid guid = Guid.NewGuid();

            // Does session cookie exist? If not, bake one!
            if (HttpContext.Session.GetString(Common.CART_COOKIE_NAME) == null)
                HttpContext.Session.SetString(Common.CART_COOKIE_NAME, guid.ToString());

            ShoppingCart shoppingCart = new ShoppingCart()
            {
                CartId = Guid.Parse(HttpContext.Session.GetString(Common.CART_COOKIE_NAME)),
                ProductId = id,
                Amount = 1
            };

            await webAPI.PostAsync<ShoppingCart>(shoppingCart, "https://localhost:44305/api/carts");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task RemoveFromCart(int id)
        {
            // Remove product from cart!!
            if (HttpContext.Session.GetString(Common.CART_COOKIE_NAME) != null)
                await webAPI.PostAsync<int>(id, "https://localhost:44305/api/carts/remove/product");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task DeleteItemFromCart(int id)
        {
            await webAPI.DeleteAsync("https://localhost:44305/api/carts/delete/" + id);
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<CartButtonInfoModel> GetCartContent()
        {
            var result = await webAPI.GetOneAsync<CartButtonInfoModel>("https://localhost:44305/api/carts/" + HttpContext.Session.GetString(Common.CART_COOKIE_NAME));
            return (result != null) ? result : new CartButtonInfoModel();
        }
    }
}