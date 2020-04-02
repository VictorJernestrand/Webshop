using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.Data;

namespace WebAPI.Models
{
    [NotMapped]
    public class AllProductsViewModel
    {

        public AllProductsViewModel()
        {
            // Do nothing
        }

        public AllProductsViewModel(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            Price = product.Price;
            Discount = product.Discount;
            DiscountPrice = product.Price - (product.Price * (decimal)product.Discount); //product.DiscountPrice;
            Quantity = product.Quantity;
            CategoryId = product.CategoryId;
            BrandId = product.BrandId;
            Description = product.Description;
            Photo = product.Photo != null ? product.Photo : "";
            BrandName = product.Brand.Name;
            CategoryName = product.Category.Name;
            //Category = product.Category;
            //Brand = product.Brand;
            FullDescription = product.FullDescription;
            Specification = product.Specification;
            ActiveProduct = product.ActiveProduct;
            
        }


       

        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public float Discount { get; set; }
        public decimal DiscountPrice { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string Description { get; set; }
        public string FullDescription { get; set; }
        public string Photo { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public Category Category { get; set; }
        public Brand Brand { get; set; }
        public string Specification { get; set; }
        public bool ActiveProduct { get; set; }

    }
}
