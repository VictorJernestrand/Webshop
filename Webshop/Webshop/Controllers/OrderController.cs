﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webshop.Context;
using Webshop.Models;
using Webshop.Services;

namespace WebAPI.Controllers
{
    public class OrderController : Controller
    {
        private readonly WebAPIHandler webAPI;
        private readonly WebAPIToken webAPIToken;
        public OrderViewModel orderviewmodel = new OrderViewModel();

        public LoggedInUserName loggedInUserName = new LoggedInUserName();

        public OrderController(WebAPIHandler webAPI, WebAPIToken webAPIToken)
        {
            this.webAPI = webAPI;
            this.webAPIToken = webAPIToken;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cartId = HttpContext.Session.GetString(Common.CART_COOKIE_NAME);

            // Does user have a shoppingcart Id?
            if (cartId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Are there any products in the cart??
            var content = await webAPI.GetAllAsync<ShoppingCartModel>(ApiURL.CARTS_CONTENT + cartId);
            if (content.Count == 0)
            {
                return RedirectToAction("Index", "ShoppingCart");
            }

            // Is user logged in?
            if (User.Identity.IsAuthenticated)
            {
                orderviewmodel = await webAPI.GetOneAsync<OrderViewModel>(ApiURL.CARTS_CONTENT_PAY + $"{cartId}/{User.Identity.Name}");

                // Does user have a complete shippingaddress?
                if (!orderviewmodel.AddressComplete)
                    TempData["Address Null"] = "Vänligen ange din adressinformation";

                // Check if order contains products out of stock
                if (orderviewmodel.Products.Any(x => x.QuantityInStock - x.Amount < 0))
                    TempData["QuantityOverload"] = "Din order innehåller ett större antal produkter än vad vi har på lager vilket påverkar leveranstiden.";
            }
            else
            {
                TempData["LoginNeeded"] = "Du måste vara inloggad för att kunna handla...";
                return RedirectToAction("Index", "ShoppingCart");
            }

            return View(orderviewmodel);
        }


        [HttpPost]
        public async Task<IActionResult> Index([Bind]OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Include the customers Email. It will be used as the userId in API
                model.UserEmail = User.Identity.Name;

                // Get cart id
                var cartId = Guid.Parse(HttpContext.Session.GetString(Common.CART_COOKIE_NAME));

                // Send order to API
                var token = await webAPIToken.New();
                var apiResult = await webAPI.PostAsync(model, ApiURL.ORDERS + cartId, token);

                if (apiResult.Status.IsSuccessStatusCode)
                    return RedirectToAction(nameof(ThankYou));
                else
                    TempData["OrderError"] = "Oops det här var pinsamt! Kunde inte skapa din order. Något sket sig, eh he hee....";

            }

            TempData["PaymentMethodError"] = "Vänligen välj ett betalsätt";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ThankYou()
        {
            User user = await webAPI.GetOneAsync<User>(ApiURL.USERS + User.Identity.Name);
            loggedInUserName.Name = user.FirstName;
            return View(loggedInUserName);
        }
        [Authorize(Roles ="Admin")]
         [HttpGet]
        public async Task<IActionResult> OrderStatus()
        {
            TestProductorder testProductorder = new TestProductorder();
 
            var token = await webAPIToken.New();
            testProductorder.productOrderViewModelslist = await webAPI.GetAllAsync<ProductOrderViewModel>(ApiURL.AllORDERS,token);
            
            return View(testProductorder);
        }
        [HttpPost]
        public async Task< IActionResult> OrderStatus([Bind]TestProductorder model)
       {
            var token = await webAPIToken.New();
            var getorder = await webAPI.GetOneAsync<Order>(ApiURL.ORDERREQBYID + model.orderid,token);


            getorder.StatusId = model.statusid;
            var updatestatuid = await webAPI.UpdateAsync<Order>(getorder, ApiURL.ORDERS + getorder.Id,token);


            return RedirectToAction("OrderStatus", "Order");
        }


        //public IActionResult OrderStatus_test()
        //{
        //    var orderlist = context.Orders.Include(x=>x.Status).ToList();
        //    List<Order> orders = new List<Order>();
        //    orders = orderlist;
        //    return View(orderlist);
        //}
        //[HttpGet]
        //public IActionResult PopulateOrderDetails(int orderid)
        //{
        //    var getstatus = context.Statuses.ToList();
        //    var result = context.ProductOrders.Include(x => x.Order)
        //                .Select(x => new ProductOrderViewModel()
        //                {
        //                    orderId = x.OrderId,
        //                    productId = x.ProductId,
        //                    Quantity = x.Amount,
        //                    price = x.Price,
        //                    orderCreationDate = x.Order.OrderDate,
        //                    orderstatus = x.Order.Status.Name,
        //                    statusId = x.Order.Status.Id,
        //                    statuslist = getstatus
        //                })
        //                .Where(x=>x.orderId==orderid)
        //                .ToList();
        //    return View(result);
        //}

        [HttpPost]
        public IActionResult PopulateOrderDetails([Bind]ProductOrderViewModel model)
        {
            return View();
        }

      //  public IActionResult 

    }
}