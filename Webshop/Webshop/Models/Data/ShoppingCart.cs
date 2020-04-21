using System;

namespace Webshop.Models.Data
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public Guid CartId { get; set; }
        public int ProductId { get; set; }
        public int Amount { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        public Product Product { get; set; }

    }
}
