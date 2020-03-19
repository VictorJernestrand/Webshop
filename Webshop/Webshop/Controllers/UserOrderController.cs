using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webshop.Context;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    public class UserOrderController : Controller
    {
        private readonly WebshopContext context;
        private readonly UserManager<User> userManager;
        OrderItemsModel orderItemsModel = new OrderItemsModel();
        OrderViewModel orderViewModel = new OrderViewModel();

        public UserOrderController(WebshopContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            // Get current logged in user
            User user = await userManager.GetUserAsync(HttpContext.User);

            var activeOrders = context.Orders.Include(x => x.Status)
                .Include(x => x.PaymentMethod)
                .Where(x => x.UserId == user.Id && x.StatusId < 4)
                .Select(x => new ActiveUserOrders
                {
                    OrderId = x.Id,
                    OrderDate = x.OrderDate,
                    OrderStatus = x.Status.Name,
                    OrderPayment = x.PaymentMethod.Name
                })
                .OrderByDescending(x => x.OrderDate)
                .ToList();


            //// Get all orders from user
            //var productOrders = context.ProductOrders.Include(x => x.Order)
            //    .Include(x => x.Product)
            //    .Where(x => x.Order.UserId == user.Id)
            //    .ToList();
            //                //.Where(x => x.CartId == cartId && x.Amount > 0)
            //                //.Select(x => new OrderItemsModel
            //                //{
            //                //    ProductId = x.Product.Id,
            //                //    ProductName = x.Product.Name,
            //                //    Photo = x.Product.Photo,
            //                //    Amount = x.Amount,
            //                //    QuantityInStock = x.Product.Quantity,
            //                //    Price = x.Product.Price,
            //                //    Discount = (decimal)x.Product.Discount,
            //                //    UnitPriceWithDiscount = CostWithDiscount(x.Product.Price, (decimal)x.Product.Discount),
            //                //    TotalProductCostDiscount = TotalCost(x.Amount, x.Product.Price, (decimal)x.Product.Discount),
            //                //    TotalProductCost = x.Product.Price * x.Amount
            //                //})
            //                //.ToList();

            return View(activeOrders);


        }

        public IActionResult OrderDetails(int id)
        {
            var orderItems = context.ProductOrders.Include(x => x.Product)
                .Where(x => x.OrderId == id)
                .Select(x => new OrderItemsModel
                {
                    ProductId = x.Product.Id,
                    ProductName = x.Product.Name,
                    Photo = x.Product.Photo,
                    Price = x.Price,
                    Amount = x.Amount,
                    Discount = x.Discount,
                    TotalProductCost = (x.Product.Price * x.Amount),
                    TotalProductCostDiscount = CalculateDiscount.NewPrice((x.Product.Price * x.Amount), (decimal)x.Product.Discount)
                })
                .ToList();

            orderViewModel.Products = orderItems;
            orderViewModel.OrderTotal = orderItems.Sum(x => x.TotalProductCostDiscount);
            return View(orderViewModel);
        }
    }
}