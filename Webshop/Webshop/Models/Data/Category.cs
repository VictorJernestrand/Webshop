using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Webshop.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori kan inte vara tomt!")]
        public string Name { get; set; }

        public IEnumerable<Category> categoryCollection { get; set; }
        //public List<Product> Products { get; set; }
        //public List<SelectedListItem> categorylist { get; set; }

    }
}
