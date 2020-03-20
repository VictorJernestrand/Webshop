using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webshop.Context;
using Webshop.Models;
using Webshop.Models.Data;
using Webshop.Services;

namespace Webshop.Controllers
{
    public class OrderController : Controller
    {
        private readonly WebshopContext context;      
        
        public OrderViewModel orderviewmodel = new OrderViewModel();
        private UserManager<User> UserMgr { get; }
       private DatabaseCRUD databaseCRUD { get; }

        public LoggedInUserName loggedInUserName = new LoggedInUserName();

        public OrderController(WebshopContext context, UserManager<User> userManager)
        {
            this.context = context;          
            this.UserMgr = userManager;
            this.databaseCRUD = new DatabaseCRUD(context);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString(Common.CART_COOKIE_NAME) == null)
            {
                // Shoppingcart is empty, send user to start page
                return RedirectToAction("Index", "Home");
            }

            if(User.Identity.IsAuthenticated)
            {
                var cartId = HttpContext.Session.GetString(Common.CART_COOKIE_NAME);
                if (cartId != null)
                {
                    // Calculate all items and discounts (if any) from shopping cart
                    var cartid = Guid.Parse(HttpContext.Session.GetString(Common.CART_COOKIE_NAME));
                    orderviewmodel.Products = GetProductDetails(orderviewmodel, Guid.Parse(cartId));

                    // Calculate total cost of whole order
                    orderviewmodel.OrderTotal = OrderTotal(orderviewmodel.Products);

                    // Get all payment methods
                    orderviewmodel.paymentMethodlist = GetPaymentMethods();

                    // Get user information from current logged in user
                    User user = await UserMgr.GetUserAsync(HttpContext.User);
                    orderviewmodel.User = user;

                    // Check if user has a complete shipping address
                    var addressComplete = false;
                    if (user.StreetAddress != null &&
                        user.PhoneNumber != null &&
                        user.ZipCode != 0 &&
                        user.City != null)
                    {
                        addressComplete = true;
                    }

                    orderviewmodel.AddressComplete = addressComplete;


                    if (orderviewmodel.User.StreetAddress==null)
                    {
                        TempData["Address Null"] = "Vänligen ange din adressinformation";
                    }

                    // Check if order contains products out of stock
                    if (orderviewmodel.Products.Any(x => x.QuantityInStock - x.Amount < 0))
                    {
                        TempData["QuantityOverload"] = "Din order innehåller ett större antal produkter än vad vi har på lager vilket påverkar leveranstiden.";
                    }


                }
                else
                {
                    //shoppingcart is empty
                }

            }
            else
            {
                TempData["LoginNeeded"] = "You need to Login,before continue shopping...";
                return RedirectToAction("Index", "ShoppingCart");
            }
            return View(orderviewmodel);
        }


        [HttpPost]
        public async Task<IActionResult> Index([Bind]OrderViewModel model)
        {
            var cartId = Guid.Parse(HttpContext.Session.GetString(Common.CART_COOKIE_NAME));

            if (ModelState.IsValid)
            {
                // Get current logged in user
                User user = await UserMgr.GetUserAsync(HttpContext.User);

                // Create order
                Order order = new Order()
                {
                    UserId = user.Id,
                    PaymentMethodId = model.PaymentMethodId,
                    StatusId = 1
                };

                // Flag for checking if order was succesful or failed miserably...
                bool idOrderSuccesful = false;

                // Keep data consistant! Begin transaction!
                using (var transaction = new System.Transactions.TransactionScope())
                {
                    //try
                    //{
                        // Add order to Entity Framework
                        context.Orders.Add(order);
                        context.SaveChanges();

                        // Get productinformation from shoppingcart
                        var productOrders = context.ShoppingCart.Include(x => x.Product)
                                                                .Where(x => x.CartId == cartId && x.Amount > 0)
                                                                .Select(x => new ProductOrder
                                                                {
                                                                    OrderId = order.Id,
                                                                    Price = x.Product.Price,
                                                                    Amount = x.Amount,
                                                                    ProductId = x.Product.Id,
                                                                    Discount = (decimal)x.Product.Discount,
                                                                })
                                                                .ToList();

                        // Add all products to the ProductOrders-table
                        context.ProductOrders.AddRange(productOrders);
                        context.SaveChanges();

                        // Update product stock/quanity in database
                        var products = context.ShoppingCart.Include(x => x.Product)
                                                           .Where(x => x.CartId == cartId).ToList();

                        foreach (var product in products)
                        {
                            product.Product.Quantity = (product.Product.Quantity - product.Amount >= 0) ?
                                                        product.Product.Quantity -= product.Amount : 0;
                            context.Products.Update(product.Product);
                            context.SaveChanges();
                        }

                        // Empty cart
                        List<ShoppingCart> cartProducts = context.ShoppingCart.Where(x => x.CartId == cartId).ToList();
                        context.ShoppingCart.RemoveRange(cartProducts);
                        context.SaveChanges();

                        // Empty shoppingcart
                        HttpContext.Session.Remove(Common.CART_COOKIE_NAME);

                        transaction.Complete();
                        idOrderSuccesful = true;
                    //}
                    //catch (Exception)
                    //{
                    //    // TODO: Transaction went wrong. Do something here...
                    //}

                    // Seems like everything went ok, set flag to true!
                    idOrderSuccesful = true;
                }

                if (idOrderSuccesful)
                    return RedirectToAction(nameof(ThankYou));
                else
                    TempData["OrderError"] = "Oops det här var pinsamt! Kunde inte skapa din order. Något sket sig, eh he hee....";
            }

            // Update model with product details from shopping cart
            model.Products = GetProductDetails(model, cartId);

            // Calculate total cost of whole order
            model.OrderTotal = OrderTotal(model.Products);

            // Update with payment methods
            model.paymentMethodlist = GetPaymentMethods();
            return View(model);
        }


        // Get order details from current cart id
        public List<OrderItemsModel> GetProductDetails(OrderViewModel orderviewmodel, Guid cartId)
        {
            var productOrders = context.ShoppingCart.Include(x => x.Product)
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


        // Caluclate discount
        private static decimal CostWithDiscount(decimal price, decimal discount)
            => price - (discount * price);

        // Calculate total itemcost
        private static decimal TotalCost(int quantity, decimal price, decimal discount)
            => CostWithDiscount(price, discount) * quantity;

        // Calculate total cost of whole order
        private static decimal OrderTotal(IEnumerable<OrderItemsModel> order)
            => order.Sum(x => x.TotalProductCostDiscount);


        // Get all available payment methods
        public List<PaymentMethod> GetPaymentMethods()
        {
            return orderviewmodel.paymentMethodlist = context.PaymentMethods.ToList();
        }


        public async Task<IActionResult> ThankYou()
        {
            User user = await UserMgr.GetUserAsync(HttpContext.User);
            loggedInUserName.Name = user.FirstName;
            return View(loggedInUserName);
        }

    }
}