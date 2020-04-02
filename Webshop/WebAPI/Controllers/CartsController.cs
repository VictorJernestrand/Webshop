using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using WebAPI.Context;
using WebAPI.Domain;
using WebAPI.Models;
using WebAPI.Models.Data;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly WebAPIContext _context;

        public CartsController(WebAPIContext context)
        {
            _context = context;
        }

        // GET: api/Cart
        [HttpGet("{id}")]
        public ActionResult<CartButtonInfoModel> GetShoppingCart(string id)
        {
            // TODO: Validate cart Id here and return an error code if id is not valid.

            Guid cartId = Guid.Parse(id);

            var cartContent = _context.ShoppingCart.Include(x => x.Product)
                .Where(x => x.CartId == cartId)
                .ToList()
                .GroupBy(x => new { x.CartId })
                .Select(x => new CartButtonInfoModel
                {
                    TotalItems = x.Sum(x => x.Amount),
                    TotalCost = x.Sum(a => CostWithDiscount((a.Product.Price * a.Amount), (decimal)a.Product.Discount)).ToString("C0")
                }).FirstOrDefault();

            return (cartContent != null) ? Ok(cartContent) : Ok(new CartButtonInfoModel());
        }


        [Route("content/{customerCartId}")]
        [HttpGet]
        public ActionResult<IEnumerable<ShoppingCartModel>> GetCartContent(string customerCartId)
        {
            // TODO: Validate cart Id here and return an error code if id is not valid.

            Guid cartId = Guid.Parse(customerCartId);

            var cartContent = _context.ShoppingCart.Include(x => x.Product)
                .Where(x => x.CartId == cartId)
                .Select(x => new ShoppingCartModel
                {
                    ShoppingCartId = x.Id,
                    ProductId = x.Product.Id,
                    Name = x.Product.Name,
                    Price = x.Product.Price,
                    DiscountPrice = CostWithDiscount(x.Product.Price, (decimal)x.Product.Discount),
                    Discount = x.Product.Discount,
                    Photo = x.Product.Photo,
                    Amount = x.Amount
                })
                .ToList();

            return Ok(cartContent);
        }

        [Route("content_and_payment/{customerCartId}/{customerEmail}")]
        [HttpGet]
        public async Task<ActionResult<OrderViewModel>> GetCartContentAndPaymentOptions(string customerCartId, string customerEmail)
        {
            Guid cartId = Guid.Parse(customerCartId);

            OrderViewModel orderViewModel = new OrderViewModel();

            // Calculate all items and discounts (if any) from shopping cart
            orderViewModel.Products = GetProductDetails(orderViewModel, cartId);

            // Calculate order total cost
            orderViewModel.OrderTotal = OrderTotal(orderViewModel.Products);

            // Get all payment methods
            orderViewModel.paymentMethodlist = await _context.PaymentMethods.ToListAsync();

            // Get user information from current logged in user
            User user = await _context.Users.Where(x => x.Email == customerEmail).FirstOrDefaultAsync();
            //orderViewModel.User = user;

            // Check if user has a complete shipping address
            var addressComplete = false;
            if (user.StreetAddress != null &&
                user.PhoneNumber != null &&
                user.ZipCode != 0 &&
                user.City != null)
            {
                addressComplete = true;
            }

            // Does user have a complete shippingaddress?
            orderViewModel.AddressComplete = addressComplete;

            return orderViewModel;
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


        public List<OrderItemsModel> GetProductDetails(OrderViewModel orderviewmodel, Guid cartId)
        {
            var productOrders = _context.ShoppingCart.Include(x => x.Product)
                .Where(x => x.CartId == cartId && x.Amount > 0)
                .Select(x => new OrderItemsModel
                {
                    ProductId = x.Product.Id,
                    ProductName = x.Product.Name,
                    Photo = x.Product.Photo,
                    Amount = x.Amount,
                    QuantityInStock = x.Product.Quantity,
                    Price = x.Product.Price,
                    Discount = (decimal)x.Product.Discount,
                    UnitPriceWithDiscount = CostWithDiscount(x.Product.Price, (decimal)x.Product.Discount),
                    TotalProductCostDiscount = TotalCost(x.Amount, x.Product.Price, (decimal)x.Product.Discount),
                    TotalProductCost = x.Product.Price * x.Amount
                })
                .ToList();

            orderviewmodel.Products = productOrders;
            return orderviewmodel.Products;
        }



        private bool ShoppingCartExists(int id)
        {
            return _context.ShoppingCart.Any(e => e.Id == id);
        }

        private static decimal CostWithDiscount(decimal price, decimal discount)
            => price - (discount * price);

        // Calculate total itemcost
        private static decimal TotalCost(int quantity, decimal price, decimal discount)
            => CostWithDiscount(price, discount) * quantity;

        // Calculate total cost of whole order
        private static decimal OrderTotal(IEnumerable<OrderItemsModel> order)
            => order.Sum(x => x.TotalProductCostDiscount);

    }
}
