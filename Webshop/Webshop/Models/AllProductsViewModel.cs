using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        
            
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Ange produktnamn")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Sätt ett pris")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Ska produkten ha en rabatt?")]
        public float Discount { get; set; }
        public decimal DiscountPrice { get; set; }

        [Required(ErrorMessage = "Ange antal")]
        public int Quantity { get; set; }

        [RegularExpression(@"[1-9](\d+)?", ErrorMessage = "Välj kategori")]
        public int CategoryId { get; set; }

        [RegularExpression(@"[1-9](\d+)?", ErrorMessage = "Välj tillverkare")]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Ange Beskrivning")]
        public string Description { get; set; }

        public string FullDescription { get; set; }

        public string Photo { get; set; }

        public string BrandName { get; set; }

        public string CategoryName { get; set; }

        public List<Category> Categories { get; set; }

        public List<Brand> Brands { get; set; }

        public List<Rating> Ratings { get; set; }

        public Rating NewRating { get; set; }

        public string Specification { get; set; }

        public bool ActiveProduct { get; set; }

        public float TotalRatingScore { get; set; }

        // public List<Product> productsDiscountlist { get; set; } = new List<Product>();

    }
}
