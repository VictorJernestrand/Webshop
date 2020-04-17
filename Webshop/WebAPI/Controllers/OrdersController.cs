using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Context;
using WebAPI.Models.Data;
using WebAPI.Models;
using WebAPI.Services;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly WebAPIContext _context;
        private readonly MailService _mailService;
        private readonly CustomerOrderService _customerOrderService;

        public OrdersController(WebAPIContext context, MailService mailService, CustomerOrderService customerOrderService)
        {
            this._context = context;
            this._mailService = mailService;
            this._customerOrderService = customerOrderService;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }


        [HttpGet("{Id}")]
        public async Task<ActionResult<Order>> GetOneOrder(int Id)
        {
            return await _context.Orders.FindAsync(Id);
        }


        [HttpGet]
        [Route("userorders/{customerEmail}")]
        public async Task<ActionResult<IEnumerable<AllUserOrders>>> GetAllUserOrderByOrderId(string customerEmail)
        {
            // Get current logged in user
            User user = await _context.Users.Where(x => x.Email == customerEmail).FirstOrDefaultAsync();

            // Return badrequest if user doesn't exit
            if (user == null)
                return BadRequest();

            // Collect orders by user id
            var activeOrders = await _context.Orders.Include(x => x.Status)
                .Include(x => x.PaymentMethod)
                .Where(x => x.UserId == user.Id)
                .Select(x => new AllUserOrders
                {
                    OrderId = x.Id,
                    OrderDate = x.OrderDate,
                    OrderStatus = x.Status.Name,
                    OrderPayment = x.PaymentMethod.Name,
                    StatusId = x.StatusId
                })
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync();

            // Orders exist?
            if (activeOrders == null)
                return NotFound();

            return Ok(activeOrders);
        }


        // GET: api/Orders/Id/5
        [Route("id/{Id}")]
        [HttpGet]
        public async Task<ActionResult<OrderViewModel>> GetOrderById(int id)
        {
            OrderViewModel orderViewModel = new OrderViewModel();
            var orderItems = await _customerOrderService.CustomerOrderByIdAsync(id);

            if (orderItems == null)
            {
                return NotFound();
            }

            orderViewModel.Id = id;
            orderViewModel.OrderStatusId = _context.Orders.Find(id).StatusId;
            orderViewModel.Products = orderItems;
            orderViewModel.OrderTotal = orderItems.Sum(x => x.TotalProductCostDiscount);

            return Ok(orderViewModel);
        }


        [Route("allorders/{statusId}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AllUserOrders>>> GetAllOrders(int statusId)
        {

            // Get all products
            var productsOrders = await _context.ProductOrders.Include(x => x.Order)
                .ThenInclude(x => x.User)
                .Where(x => x.Order.StatusId == statusId)
                .Select(x => new AllUserOrders
                 {
                     OrderId = x.Order.Id,
                     OrderDate = x.Order.OrderDate,
                     StatusId = x.Order.StatusId,
                     CustomerName = x.Order.User.FirstName + " " + x.Order.User.LastName,
                     CustomerEmail = x.Order.User.Email,
                     Quantity = x.Amount,
                     TotalCost = CalculateDiscount.NewPrice(x.Price * x.Amount, x.Discount)
                })
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync();

            // Group orders by OrderId
            var orders = productsOrders.GroupBy(x => x.OrderId).Select(x => new AllUserOrders
            {
                OrderId = x.First().OrderId,
                OrderDate = x.First().OrderDate,
                StatusId = x.First().StatusId,
                CustomerName = x.First().CustomerName,
                CustomerEmail = x.First().CustomerEmail,
                Quantity = x.Sum(s => s.Quantity),
                TotalCost = x.Sum(s => s.TotalCost)
            }).ToList();

            return Ok(orders);
        }

        /*
        [Route("orderitems/{orderId}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItemsModel>>> GetOrderItemsByOrderId(int orderId)
        {
            var test = _context.Orders.Find(orderId);

            try
            {
                var orderItems = await _context.ProductOrders.Include(x => x.Product)
                    .Where(x => x.OrderId == orderId)
                    .Select(x => new OrderItemsModel
                    {
                        ProductId = x.Product.Id,
                        ProductName = x.Product.Name,
                        Photo = x.Product.Photo,
                        Price = x.Price,
                        Amount = x.Amount,
                        Discount = x.Discount,
                        QuantityInStock = x.Product.Quantity,
                        TotalProductCost = (x.Price * x.Amount),
                        TotalProductCostDiscount = CalculateDiscount.NewPrice((x.Price * x.Amount), x.Discount)
                    })
                    .ToListAsync();

                return Ok(orderItems);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
        */

        [Route("orderrequest/{id}")]
        [HttpGet]
        public async Task<Order> GetAdminOrderById(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Orders
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost("{Id}")]
        public async Task<ActionResult<Order>> PostOrder(string Id, OrderViewModel orderView)
        {
            // Cart Id
            var cartId = Guid.Parse(Id);

            User user = await _context.Users.AsNoTracking()
                .Where(x => x.Email == orderView.UserEmail)
                .FirstOrDefaultAsync();

            // Create order
            Order newOrder = new Order()
            {
                UserId = user.Id,
                PaymentMethodId = orderView.PaymentMethodId,
                StatusId = 1
            };

            try
            {
                // Keep data consistant! Begin transaction!
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Add order to Entity Framework
                    _context.Orders.Add(newOrder);
                    await _context.SaveChangesAsync();

                    // Get productinformation from shoppingcart
                    var productOrders = await _context.ShoppingCart.Include(x => x.Product)
                        .Where(x => x.CartId == cartId && x.Amount > 0)
                        .Select(x => new ProductOrder
                        {
                            OrderId = newOrder.Id,
                            Price = x.Product.Price,
                            Amount = x.Amount,
                            ProductId = x.Product.Id,
                            Discount = (decimal)x.Product.Discount,
                        })
                        .ToListAsync();

                    // Add all products to the ProductOrders-table
                    _context.ProductOrders.AddRange(productOrders);
                    await _context.SaveChangesAsync();

                    // Update product quantity
                    foreach (var item in productOrders)
                    {
                        var product = _context.Products.Find(item.ProductId);

                        product.Quantity = (product.Quantity - item.Amount >= 0) ?
                                            product.Quantity -= item.Amount : 0;

                        _context.Entry(product).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }

                    // Empty cart.
                    List<ShoppingCart> cartProducts = await _context.ShoppingCart.Where(x => x.CartId == cartId).ToListAsync();
                    _context.ShoppingCart.RemoveRange(cartProducts);
                    _context.SaveChanges();

                    // Keep cart clean by removing carts older than 3 days
                    List<ShoppingCart> oldCarts = await _context.ShoppingCart.Where(x => x.TimeStamp.AddDays(3) <= DateTime.Now).ToListAsync();
                    _context.ShoppingCart.RemoveRange(oldCarts);
                    _context.SaveChanges();

                    // Send order-confirmation mail to customer
                    //MailService mail = new MailService();
                    bool test = await _mailService.SendOrderConfirmationMailAsync(user.Email, user.FirstName, "Orderbekräftelse", newOrder.Id);

                    // Flag that the SQL-transaction has completed successfully
                    transaction.Complete();

                    return CreatedAtAction("GetOrderById", new { id = newOrder.Id }, newOrder);
                }
            }
            catch (Exception ex)
            {
                return new Order();
            }
        }

        // DELETE: api/Orders/5C:\Users\Nick\source\repos\Webshop\Webshop\WebAPI\Properties\
        [HttpDelete("{id}")]
        public async Task<ActionResult<Order>> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return order;
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

    }
}
