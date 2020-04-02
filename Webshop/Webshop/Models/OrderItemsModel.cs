using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class OrderItemsModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string Photo { get; set; }

        public int Amount { get; set; }

        public int QuantityInStock { get; set; }

        public decimal ProductPrice { get; set; }

        public decimal Price { get; set; }

        public decimal Discount { get; set; }

        public decimal UnitPriceWithDiscount { get; set; }

        public decimal TotalProductCostDiscount { get; set; }

        public decimal TotalProductCost { get; set; }

      
        

    }
}
