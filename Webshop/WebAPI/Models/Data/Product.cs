using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models.Data
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string Description { get; set; }
        public string FullDescription { get; set; }
        public string Photo { get; set; }
        public float Discount { get; set; }
        public string Specification { get; set; }
        public Category Category { get; set; }
        public Brand Brand { get; set; }
        public List<ProductOrder> ProductOrders { get; set; }


    }
}
