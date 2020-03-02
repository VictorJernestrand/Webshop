using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class EditBrandModel
    {
        public IEnumerable<Brand> BrandsCollection { get; set; }
        
        public Brand Brand { get; set; }
    }
}
