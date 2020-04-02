using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Context;
using WebAPI.Models;

namespace WebAPI.Services
{
    public class CustomerOrderService
    {
        private readonly WebAPIContext _context;

        public CustomerOrderService(WebAPIContext _context)
        {
            this._context = _context;
        }

        public async Task<List<OrderItemsModel>> CustomerOrderByIdAsync(int orderId)
        {
            //OrderViewModel orderViewModel = new OrderViewModel();
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
                        TotalProductCost = (x.Price * x.Amount),
                        TotalProductCostDiscount = CalculateDiscount.NewPrice((x.Price * x.Amount), x.Discount)
                    })
                    .ToListAsync();

                return orderItems;
            }
            catch (Exception ex)
            {
                //
            }

            return null;
        }
    }
}
