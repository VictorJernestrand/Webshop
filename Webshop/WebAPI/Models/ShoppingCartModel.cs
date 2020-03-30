using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class ShoppingCartModel
    {
        public int ShoppingCartId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public float Discount { get; set; }
        public decimal DiscountPrice { get; set; }
        public string Photo { get; set; }
        public int Amount { get; set; }
    }
}
