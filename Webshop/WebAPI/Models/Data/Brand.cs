using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models.Data
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Product> Products { get; set; }


    }
}
