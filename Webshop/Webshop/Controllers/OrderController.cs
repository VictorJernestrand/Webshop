using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Webshop.Models;
using Webshop.Services;

namespace WebAPI.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly WebAPIHandler webAPI;
        private readonly WebAPIToken webAPIToken;

        private readonly string _cartSessionCookie;

        public OrderViewModel orderviewmodel = new OrderViewModel();
        public OrderAndPaymentMethods OrderAndPaymentMethods = new OrderAndPaymentMethods();

        

        //public OrderViewModel orderviewmodel = new OrderViewModel();
        public LoggedInUserName loggedInUserName = new LoggedInUserName();

        public OrderController(WebAPIHandler webAPI, WebAPIToken webAPIToken, IConfiguration config)
        {
            this.webAPI = webAPI;
            this.webAPIToken = webAPIToken;
            this._cartSessionCookie = config["CartSessionCookie:Name"];
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cartId = HttpContext.Session.GetString(_cartSessionCookie);

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

            OrderAndPaymentMethods.OrderViewModel = orderviewmodel;

            var token = await webAPIToken.New();
            OrderAndPaymentMethods.User = await webAPI.GetOneAsync<User>(ApiURL.USERS + User.Identity.Name, token);
            return View(OrderAndPaymentMethods);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditStatus(int Id)
        {
            var token = await webAPIToken.New();
            var orderItems = await webAPI.GetOneAsync<OrderViewModel>(ApiURL.ORDER_BY_ID + Id, token);

            orderItems.Statuses = await webAPI.GetAllAsync<Status>(ApiURL.STATUS);
            return View(orderItems );

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditStatus([Bind]OrderViewModel model, int statusId)
        {
            var token = await webAPIToken.New();
            var order = await webAPI.GetOneAsync<Order>(ApiURL.ORDERS + model.Id, token);

            // Update status
            order.StatusId = statusId;

            // Save changes
            var apiResult = await webAPI.UpdateAsync(order, ApiURL.ORDERS + model.Id, token);

            TempData["StatusUpdate"] = true;
            return RedirectToAction("EditStatus");
        }

        [HttpPost]
        public async Task<IActionResult> Index([Bind]OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Include customers Email. It will be used as userId in the API
                model.UserEmail = User.Identity.Name;

                // Get cart id
                var cartId = Guid.Parse(HttpContext.Session.GetString(_cartSessionCookie));

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


        [HttpPost, ActionName("CreditCardPayment")]
        [ValidateAntiForgeryToken]
        public IActionResult PostCreditCardPayment([Bind]CreditCardModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CreditCardModel creditCardPayment = new CreditCardModel()
                    {
                        CardNumber = model.CardNumber,
                        CVC = model.CVC
                    };

                    return RedirectToAction(nameof(ThankYou));
                }
                return View(model);
            }
            catch
            {
                return View();
            }
        }
        //public IActionResult CreditCardPayment()
        //{

        //    return View(creditCardModel);
        //}


        public async Task<IActionResult> ThankYou()
        {
            User user = await webAPI.GetOneAsync<User>(ApiURL.USERS + User.Identity.Name);
            loggedInUserName.Name = user.FirstName;
            return View(loggedInUserName);
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<IActionResult> OrderStatus(int? statusId)
        {
            if (statusId == null)
                statusId = 1;

            ProductOrderViewModel orders = new ProductOrderViewModel();
            orders.Statuses = await webAPI.GetAllAsync<Status>(ApiURL.STATUS);

            var token = await webAPIToken.New();
            orders.Orders = await webAPI.GetAllAsync<AllUserOrders>(ApiURL.All_ORDERS_BY_STATUS + statusId, token);

            return View(orders);
        }

    }
}