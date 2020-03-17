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

        public OrderController(WebshopContext context, UserManager<User> userManager)
        {
            this.context = context;          
            this.UserMgr = userManager;
            this.databaseCRUD = new DatabaseCRUD(context);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if(User.Identity.IsAuthenticated)
            {
                var cartId = HttpContext.Session.GetString("CustomerCartSessionId");
                if (cartId != null)
                {
                    // Get all products from shopping cart
                    var cartid = Guid.Parse(HttpContext.Session.GetString(Common.CART_COOKIE_NAME));
                    orderviewmodel.Products = GetProductDetails(orderviewmodel, Guid.Parse(cartId));

                    // Get all payment methods
                    orderviewmodel.paymentMethodlist = GetPaymentMethods();

                    User user = await UserMgr.GetUserAsync(HttpContext.User);

                    if(user.StreetAddress==null)
                    {
                        TempData["Address Null"] = "You haven't updated your Address book";
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
                User user = await UserMgr.GetUserAsync(HttpContext.User);


                //user id is not in Order table
                Order order = new Order()
                {
                    UserId = user.Id,
                    PaymentMethodId = model.PaymentMethodId,
                    StatusId = 1

                };

                var result = await databaseCRUD.InsertAsync<Order>(order);

                //orderviewmodel = GetOrderDetails(new OrderViewModel(), cartid);

                var query = context.ShoppingCart.Where(x => x.CartId == cartId).ToList();

                ProductOrder productOrder = new ProductOrder();
                int output = 0;
                foreach (var item in query)
                {
                    //var itemPrice = context.Products.Where(x => x.Id == item.ProductId).Select(x => x.Price);
                    var productItems = context.Products.Where(x => x.Id == item.ProductId).ToList();

                    foreach (var product in productItems)
                    {
                        if (product.Quantity == 0)
                        {
                            // Do something here...

                        }

                        productOrder.Amount = Convert.ToInt32(item.Amount * product.Price);
                    }

                    productOrder.OrderId = order.Id;
                    productOrder.ProductId = item.ProductId;
                    productOrder.Discount = 0;
                    output = await databaseCRUD.InsertAsync<ProductOrder>(productOrder);
                    productOrder.Id = 0;


                }

                if (result > 0 && output > 0)
                {
                    TempData["OrderCreated"] = "Your order successfully created";
                }

                // Empty cart from all items
                EmptyCart(cartId);


                return RedirectToAction("AllProducts", "Product");
            }

            // Update model with product details from shopping cart
            model.Products = GetProductDetails(model, cartId);

            // Update with payment methods
            model.paymentMethodlist = GetPaymentMethods();
            return View(model);
        }


        // Get order details from current cart id
        public List<Product> GetProductDetails(OrderViewModel orderviewmodel, Guid cartId)
        {
            orderviewmodel.Products = new List<Product>();
            var cartItems = context.ShoppingCart.Where(x => x.CartId == cartId).ToList();
            foreach (var item in cartItems)
            {

                var product = context.Products.Find(item.ProductId);
                orderviewmodel.Products.Add(product);
            }

            return orderviewmodel.Products;
        }


        // Get all available payment methods
        public List<PaymentMethod> GetPaymentMethods()
        {
            return orderviewmodel.paymentMethodlist = context.PaymentMethods.ToList();
        }


        // Empty cart by session id
        public void EmptyCart(Guid cartId)
        {
            // TODO: Emppty cart in database based on the cartId

            //List<ShoppingCart> items = context.ShoppingCart.Where(x => x.CartId == cartId).ToList();
            //foreach(var item in items)
            //{
            //    context.Remove<ShoppingCart>(item);
            //    context.SaveChanges();
            //}

            HttpContext.Session.Remove(Common.CART_COOKIE_NAME);
        }

    }
}