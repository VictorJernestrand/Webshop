using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Webshop.Models
{
    public class Brand
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tillverkare kan inte vara tomt!")]
        public string Name { get; set; }

        public IEnumerable<Brand> BrandsCollection { get; set; }

        //public List<Product> Products { get; set; }


    }
}
