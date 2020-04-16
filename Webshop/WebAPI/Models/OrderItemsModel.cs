using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.Data;

namespace WebAPI.Models
{
    public class OrderItemsModel
    {
        public int ShoppingCartId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string Photo { get; set; }

        public int Amount { get; set; }

        public int QuantityInStock { get; set; }

        public decimal ProductPrice { get; set; }

        public decimal Price { get; set; }

        public float Discount { get; set; }

        public decimal DiscountPrice { get; set; }

        public decimal UnitPriceWithDiscount { get; set; }

        public decimal TotalProductCostDiscount { get; set; }

        public decimal TotalProductCost { get; set; }

    }
}
