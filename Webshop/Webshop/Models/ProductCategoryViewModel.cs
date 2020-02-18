using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class ProductCategoryViewModel
    {
        public SelectList catlist { get; set; }
        public Category selectedcategory { get; set; }
    }
}
