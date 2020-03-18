using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models.Data
{
    public class ProductOrder
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public float Discount { get; set; }
        public int Amount { get; set; }
        public Product Product { get; set; }
        public Order Order { get; set; }


    }
}
