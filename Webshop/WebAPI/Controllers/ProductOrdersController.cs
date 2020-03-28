using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Context;
using WebAPI.Models;
using WebAPI.Models.Data;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductOrdersController : ControllerBase
    {/*
        private readonly WebAPIContext _context;

        public ProductOrdersController(WebAPIContext context)
        {
            _context = context;
        }

        // GET: api/ProductOrders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductOrder>>> GetProductOrder()
        {
            return await _context.ProductOrders.ToListAsync();
        }

        // GET: api/ProductOrders/45345-345345-345345
        [HttpGet("{id}/{customerEmail}")]
        public async Task<ActionResult<OrderViewModel>> GetProductOrderDetailsFromCart(string id, string customerEmail)
        {
            Guid cartId = Guid.Parse(id);

            OrderViewModel orderViewModel = new OrderViewModel();

            // Calculate all items and discounts (if any) from shopping cart
            orderViewModel.Products = GetProductDetails(orderViewModel, cartId);

            // Get all payment methods
            orderViewModel.paymentMethodlist = await _context.PaymentMethods.ToListAsync();// await webAPI.GetAllAsync<PaymentMethod>("https://localhost:44305/api/payment/");

            // Get user information from current logged in user
            User user = await _context.Users.Where(x => x.Email == customerEmail).FirstOrDefaultAsync();
            orderViewModel.User = user;

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

        // PUT: api/ProductOrders/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutProductOrder(int id, ProductOrder productOrder)
        //{
        //    if (id != productOrder.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(productOrder).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!ProductOrderExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/ProductOrders
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<ProductOrder>> PostProductOrder(ProductOrder productOrder)
        {
            _context.ProductOrders.Add(productOrder);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductOrder", new { id = productOrder.Id }, productOrder);
        }

        // DELETE: api/ProductOrders/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductOrder>> DeleteProductOrder(int id)
        {
            var productOrder = await _context.ProductOrders.FindAsync(id);
            if (productOrder == null)
            {
                return NotFound();
            }

            _context.ProductOrders.Remove(productOrder);
            await _context.SaveChangesAsync();

            return productOrder;
        }

        private bool ProductOrderExists(int id)
        {
            return _context.ProductOrders.Any(e => e.Id == id);
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

        private static decimal CostWithDiscount(decimal price, decimal discount)
            => price - (discount * price);

        // Calculate total itemcost
        private static decimal TotalCost(int quantity, decimal price, decimal discount)
            => CostWithDiscount(price, discount) * quantity;

        // Calculate total cost of whole order
        private static decimal OrderTotal(IEnumerable<OrderItemsModel> order)
            => order.Sum(x => x.TotalProductCostDiscount);
*/
    }
}
