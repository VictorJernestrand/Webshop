using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using WebAPI.Context;
using WebAPI.Models;
using WebAPI.Models.Data;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly WebAPIContext _context;

        public CartController(WebAPIContext context)
        {
            _context = context;
        }

        // GET: api/Cart
        [HttpGet("{id}")]
        public ActionResult<CartButtonInfoModel> GetShoppingCart(string id)
        {
            Guid cartId = Guid.Parse(id);

            var cartContent = _context.ShoppingCart.Include(x => x.Product)
                .Where(x => x.CartId == cartId)
                .ToList()
                .GroupBy(x => new { x.CartId })
                .Select(x => new CartButtonInfoModel
                {
                    TotalItems = x.Sum(x => x.Amount),
                    TotalCost = x.Sum(a => NewPrice((a.Product.Price * a.Amount), (decimal)a.Product.Discount)).ToString("C0")
                }).FirstOrDefault();

            return (cartContent != null) ? Ok(cartContent) : Ok(new CartButtonInfoModel());
        }


        [Route("content/{customerCartId}")]
        [HttpGet]
        public ActionResult<IEnumerable<CartButtonInfoModel>> Renew(string customerCartId)
        {
            Guid cartId = Guid.Parse(customerCartId);

            var cartContent = _context.ShoppingCart.Include(x => x.Product)
                .Where(x => x.CartId == cartId)
                .Select(x => new ShoppingCartModel
                {
                    ShoppingCartId = x.Id,
                    ProductId = x.Product.Id,
                    Name = x.Product.Name,
                    Price = x.Product.Price,
                    DiscountPrice = NewPrice(x.Product.Price, (decimal)x.Product.Discount),
                    Discount = x.Product.Discount,
                    Photo = x.Product.Photo,
                    Amount = x.Amount
                })
                .ToList();

            return Ok(cartContent);
        }


        // POST: api/Cart
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<ShoppingCart>> PostShoppingCart(ShoppingCart shoppingCart)
        {
            // Get product form database
            Product product = _context.Products.Find(shoppingCart.ProductId);

            // Cart Id
            Guid cartId = shoppingCart.CartId;

            ShoppingCart cart = new ShoppingCart();

            // Does product allready exist in shoppingcart??
            var cartItem = _context.ShoppingCart.Where(x => x.CartId == cartId && x.ProductId == product.Id).FirstOrDefault();

            if (cartItem != null)
            {
                cartItem.Amount++;
            }
            else
            {
                // Instantiate a new schoppingcart SQ:-model with the selected prodcut id
                cart = new ShoppingCart()
                {
                    CartId = cartId,
                    ProductId = shoppingCart.ProductId,
                    Amount = 1
                };

                // All good, put item in shoppingcart
                _context.Add<ShoppingCart>(cart);
            }

            // Update timestamp 
            List<ShoppingCart> carts = await _context.ShoppingCart.Where(x => x.CartId == cartId).ToListAsync();
            carts.ForEach(x => x.TimeStamp = DateTime.Now);

            _context.SaveChanges();

            return Ok();

        }


        [Route("remove/product")]
        [HttpPost]
        public async Task<ActionResult> RemoveProductFromCart([FromBody]int id)
        {
            // Get item from shoppingcart to be removed
            var cartProductItem = await _context.ShoppingCart.FindAsync(id);

            // Is there anything to be removed?
            if (cartProductItem.Amount > 0)
            {
                // Remove item from cart
                cartProductItem.Amount--;
                _context.SaveChanges();
            }

            return Ok();
        }


        // DELETE: api/Cart/5
        [Route("delete/{id}")]
        [HttpDelete]
        public async Task<ActionResult<ShoppingCart>> DeleteProductFromCart(int id)
        {
            var shoppingCart = await _context.ShoppingCart.FindAsync(id);
            if (shoppingCart == null)
            {
                return NotFound();
            }

            _context.ShoppingCart.Remove(shoppingCart);
            await _context.SaveChangesAsync();

            return Ok(shoppingCart);
        }



        private bool ShoppingCartExists(int id)
        {
            return _context.ShoppingCart.Any(e => e.Id == id);
        }


        public static decimal NewPrice(decimal price, decimal discount)
        {
            return price - (price * discount);
        }

    }
}
