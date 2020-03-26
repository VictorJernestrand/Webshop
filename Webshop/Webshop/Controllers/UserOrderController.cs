using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webshop.Context;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    [Authorize]
    public class UserOrderController : Controller
    {
        private readonly WebshopContext context;
        private readonly WebAPIHandler webAPI;
        //private readonly UserManager<User> userManager;
        //OrderItemsModel orderItemsModel = new OrderItemsModel();
        //OrderViewModel orderViewModel = new OrderViewModel();

        //<<<<<<< HEAD
        //public UserOrderController(WebshopContext context, UserManager<User> userManager)
        //{
        //    this.context = context;
        //    this.userManager = userManager;
        //}

        public UserOrderController(WebshopContext context, WebAPIHandler webAPI)
        {
            this.context = context;
            this.webAPI = webAPI;
        }

        public async Task<IActionResult> Index()
        {
            var ewtwt = User.Identity.Name;

            // Get current logged in user
            User user = await context.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefaultAsync();// await userManager.GetUserAsync(HttpContext.User);
            //User user = await webAPI.GetOneAsync<User>("https://localhost:44305/api/User/" + User.Identity.Name);

            var allOrders = context.Orders.Include(x => x.Status)
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
                .ToList();


        

            return View(allOrders);


        }

        public async Task<ActionResult> OrderDetails(int id)
        {
            //var orderItems = context.ProductOrders.Include(x => x.Product)
            //    .Where(x => x.OrderId == id)
            //    .Select(x => new OrderItemsModel
            //    {
            //        ProductId = x.Product.Id,
            //        ProductName = x.Product.Name,
            //        Photo = x.Product.Photo,
            //        Price = x.Price,
            //        Amount = x.Amount,
            //        Discount = x.Discount,
            //        TotalProductCost = (x.Product.Price * x.Amount),
            //        TotalProductCostDiscount = CalculateDiscount.NewPrice((x.Product.Price * x.Amount), (decimal)x.Product.Discount)
            //    })
            //    .ToList();

            //orderViewModel.Products = orderItems;
            //orderViewModel.OrderTotal = orderItems.Sum(x => x.TotalProductCostDiscount);

            var orderDetails = await webAPI.GetOneAsync<OrderViewModel>("https://localhost:44305/api/orders/" + id);

            return View(orderDetails);
            //return View(orderViewModel);
        }
    }
}