using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class EditProductModel
    {
        public EditProductModel()
        {

        }

        public EditProductModel(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            Price = product.Price;
            Quantity = product.Quantity;
            CategoryId = product.CategoryId;
            BrandId = product.BrandId;
            Description = product.Description;
            Photo = product.Photo != null ? product.Photo : "";
            
        }
        public int Id { get; set; }

        [Required(ErrorMessage = "Ange Produkt Namn")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Ange Produkt Pris")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Ange Antal")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Ange Kategori Id")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Ange Brand Id")]
        public int BrandId { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Välje en fil")]
        public string Photo { get; set; }
        public List<Product> products { get; set; }
        public List<Category> categoryVM { get; set; } = new List<Category>();
        public List<Brand> brandVM { get; set; } = new List<Brand>();
    }
}
