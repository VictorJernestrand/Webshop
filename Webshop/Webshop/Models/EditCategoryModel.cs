using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Webshop.Models
{
    public class EditCategoryModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori kan inte vara tomt!")]
        public string Name { get; set; }

        public IEnumerable<Category> categoryCollection { get; set; }

    }
}
