using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webshop.Context;
using Webshop.Models;
using Webshop.Services;
using Webshop.Models.Data;

namespace Webshop.Controllers
{
    public class ShoppingCartController : Controller
    {
        // SQL connection
        private readonly WebshopContext _context;

        AllProductsViewModel allProducts;

        public ShoppingCartController(WebshopContext context)
        {
            this.allProducts = new AllProductsViewModel();
            this._context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("CustomerCartSessionId") != null)
            {
                // Parse session cookie to Guid
                Guid cartId = Guid.Parse(HttpContext.Session.GetString("CustomerCartSessionId"));

                // Get cart contents from database
                var cartContent = _context.ShoppingCart.Include(x => x.Product)
                    .Where(x => x.CartId == cartId)
                    .Select(x => new ShoppingCartModel
                    {
                        ShoppingCartId = x.Id,
                        ProductId = x.Product.Id,
                        Name = x.Product.Name,
                        Price = x.Product.Price,
                        Photo = x.Product.Photo,
                        Amount = x.Amount
                    })
                    .ToList();

                return View(cartContent);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void AddToCart(int id)
        {
            // Generate a unique id
            Guid guid = Guid.NewGuid();

            // Check if session cookie exist
            if (HttpContext.Session.GetString(Common.CART_COOKIE_NAME) == null)
            {
                // Set session cookie with Guid Id
                HttpContext.Session.SetString(Common.CART_COOKIE_NAME, guid.ToString());
            }

            // Get product form database
            Product product = _context.Products.Find(id);

            // Cart Id
            Guid cartId = Guid.Parse(HttpContext.Session.GetString(Common.CART_COOKIE_NAME));

            // Does product allready exist in shoppingcart??
            var cartItem = _context.ShoppingCart.Where(x => x.CartId == cartId && x.ProductId == id).FirstOrDefault();
            if (cartItem != null)
            {
                cartItem.Amount++; // add one more
            }
            else
            {
                // Instantiate a new schoppingcart SQ:-model with the selected prodcut id
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    CartId = cartId,
                    ProductId = id,
                    Amount = 1
                };

                // All good, put item in shoppingcart
                _context.Add<ShoppingCart>(shoppingCart);
            }

            // Update timestamp 
            List<ShoppingCart> carts = _context.ShoppingCart.Where(x => x.CartId == cartId).ToList();
            carts.ForEach(x => x.TimeStamp = DateTime.Now);

            // save, fool!
            _context.SaveChanges();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void RemoveFromCart(int id)
        {
            // Is there a session cookie? Remove product from cart!!
            if (HttpContext.Session.GetString(Common.CART_COOKIE_NAME) != null)
            {
                // Get item from shoppingcart to be removed
                var cartProductItem = _context.ShoppingCart.Find(id);

                // Are there anything to be removed?
                if (cartProductItem.Amount > 0)
                {
                    // Remove item from cart
                    cartProductItem.Amount--;

                    // Update database
                    _context.SaveChanges();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void DeleteItemFromCart(int id)
        {
            var cartItem = _context.ShoppingCart.Find(id);
            _context.Remove(cartItem);
            _context.SaveChanges();
        }

        [HttpGet]
        [Produces("application/json")]
        public CartButtonInfoModel GetCartContent()
        {
            // Is there a session cookie?
            if (HttpContext.Session.GetString(Common.CART_COOKIE_NAME) == null)
                return new CartButtonInfoModel();

            Guid cartId = Guid.Parse(HttpContext.Session.GetString(Common.CART_COOKIE_NAME));

            var cartContent = _context.ShoppingCart.Include(x => x.Product)
                .Where(x => x.CartId == Guid.Parse(HttpContext.Session.GetString(Common.CART_COOKIE_NAME)))
                .ToList()
                .GroupBy(x => new { x.CartId })
                .Select(x => new CartButtonInfoModel
                {
                    TotalItems = x.Sum(x => x.Amount),

                    // Result = Total - (Discount * Total)
                    TotalCost = x.Sum(a => (a.Product.Price * a.Amount) - ((decimal)a.Product.Discount * (a.Product.Price * a.Amount))).ToString("C0")
                }).FirstOrDefault();

            // If cartContent is null return new CartButtonmodel with default values
            return (cartContent != null) ? cartContent : new CartButtonInfoModel();
        }
    }
}