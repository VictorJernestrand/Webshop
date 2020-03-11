using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
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
            Quantity = product.Quantity;
            CategoryId = product.CategoryId;
            BrandId = product.BrandId;
            Description = product.Description;
            Photo = product.Photo != null ? product.Photo : "";
            BrandName = product.Brand.Name;
            CategoryName = product.Category.Name;
            Category = product.Category;
            Brand = product.Brand;
            FullDescription = product.FullDescription;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
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

    }
}
