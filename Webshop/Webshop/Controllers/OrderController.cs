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
            // TODO: Get all products from cart and display all products user wants to by
            // Also show what items are in stock and ask user how the sucker wants to proceed...

            if(User.Identity.IsAuthenticated)
            {
                var cartId = HttpContext.Session.GetString("CustomerCartSessionId");
                if (cartId != null)
                {
                    //var cartid = Guid.Parse(HttpContext.Session.GetString("CustomerCartSessionId"));
                    //orderviewmodel.shoppinglist = context.ShoppingCart.Where(x => x.CartId == cartid).ToList();
                 
                    orderviewmodel.paymentMethodlist = context.PaymentMethods.ToList();

                    orderviewmodel.Products = new List<Product>();
                    var cartItems = context.ShoppingCart.Where(x => x.CartId == Guid.Parse(cartId)).ToList();
                    foreach (var item in cartItems)
                    {

                        var product = context.Products.Find(item.ProductId);
                        orderviewmodel.Products.Add(product);

                        //foreach(var product in )
                        //orderviewmodel.Products.Add( new Product
                        //{
                        //    Name = product.Product.Name,
                        //    Quantity = product.Product.Quantity
                        //});
                    }
                  
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
            User user = await UserMgr.GetUserAsync(HttpContext.User);
           

            
            Order order = new Order()
            {
                UserId=user.Id,
                PaymentMethodId=model.PaymentMethodId,
                StatusId=1
                
            };

            

            var result = await databaseCRUD.InsertAsync<Order>(order);

            var cartid = Guid.Parse(HttpContext.Session.GetString("CustomerCartSessionId"));
            var query = context.ShoppingCart.Where(x => x.CartId == cartid).ToList();

           
            ProductOrder productOrder = new ProductOrder();
            int output = 0;
            foreach (var item in query)
            {

                //take quantity along with price from product table
                var itemPrice = context.Products.Where(x => x.Id == item.ProductId).Select(x => x.Price);

                foreach (var price in itemPrice)
                {
                    //if price.quantity is > item.ammount
                    productOrder.Amount = Convert.ToInt32(item.Amount * price);
                    //else
                    //Intimate the user of late delivery and he wishes to continue??
                }
              
                productOrder.OrderId = order.Id;
                productOrder.ProductId = item.ProductId;
                productOrder.Discount = 0;
                 output=  await databaseCRUD.InsertAsync<ProductOrder>(productOrder);

                //Update the database with reduced amount
                productOrder.Id = 0;


            }



            if (result>0&&output>0)
            {
                TempData["OrderCreated"] = "Your order successfully created";
            }
            return RedirectToAction("AllProducts","Product");
        }

    }
}