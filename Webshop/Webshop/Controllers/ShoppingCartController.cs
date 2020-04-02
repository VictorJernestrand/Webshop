using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webshop.Models;
using Webshop.Services;
using Webshop.Models.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Webshop.Controllers
{
    public class ShoppingCartController : Controller
    {
        // SQL connection
        private readonly WebAPIHandler webAPI;
        private string _cartSessionCookie;

        public ShoppingCartController(WebAPIHandler webAPI, IConfiguration config)
        {
            this.webAPI = webAPI;
            this._cartSessionCookie = config["CartSessionCookie:Name"];
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString(_cartSessionCookie) != null)
            {
                var cartId = HttpContext.Session.GetString(_cartSessionCookie);
                var result = await webAPI.GetAllAsync<ShoppingCartModel>("https://localhost:44305/api/carts/content/" + cartId);
                return View(result);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int id)
        {
            // Generate a unique id
            Guid guid = Guid.NewGuid();

            // Does session cookie exist? If not, bake one!
            if (HttpContext.Session.GetString(_cartSessionCookie) == null)
                HttpContext.Session.SetString(_cartSessionCookie, guid.ToString());

            ShoppingCart shoppingCart = new ShoppingCart()
            {
                CartId = Guid.Parse(HttpContext.Session.GetString(_cartSessionCookie)),
                ProductId = id,
                Amount = 1
            };

            await webAPI.PostAsync<ShoppingCart>(shoppingCart, "https://localhost:44305/api/carts");
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            // Remove product from cart!!
            if (HttpContext.Session.GetString(_cartSessionCookie) != null)
                await webAPI.PostAsync<int>(id, "https://localhost:44305/api/carts/remove/product");
                
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItemFromCart(int id)
        {
            await webAPI.DeleteAsync("https://localhost:44305/api/carts/delete/" + id);
            return Ok();
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<CartButtonInfoModel> GetCartContent()
        {
            var result = await webAPI.GetOneAsync<CartButtonInfoModel>("https://localhost:44305/api/carts/" + HttpContext.Session.GetString(_cartSessionCookie));
            return (result != null) ? result : new CartButtonInfoModel();
        }
    }
}