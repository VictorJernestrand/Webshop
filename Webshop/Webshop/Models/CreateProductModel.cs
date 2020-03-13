using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    
    public class CreateProductModel
    {
       public int Id { get; set; }

        [Required(ErrorMessage = "Ange Produkt Namn")]
        
        public string Name { get; set; }
        
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Ange Antal")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Ange Kategori Id")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Ange Brand Id")]
        public int BrandId { get; set; }

        public string Description { get; set; }
        public string FullDescription { get; set; }
        public string Photo { get; set; }
        public List<Category> categoryVM { get; set; } = new List<Category>();
        public List<Brand> brandVM { get; set; } = new List<Brand>();

        [Required(ErrorMessage = "Ange Produkt Pris")]
        public string PriceToConvert { get; set; }

        public string Specification { get; set; }

        internal Task CopyToAsync(FileStream fileStream)
        {
            throw new NotImplementedException();
        }
    }
}
