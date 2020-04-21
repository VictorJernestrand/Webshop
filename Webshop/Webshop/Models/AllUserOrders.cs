using System;

namespace Webshop.Models
{
    public class AllUserOrders
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public string OrderPayment { get; set; }
        public int StatusId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public int Quantity { get; set; } = 0;
        public decimal TotalCost { get; set; } = 0;

    }
}
