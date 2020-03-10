using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webshop.Context;
using Webshop.Models;

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
    }
}