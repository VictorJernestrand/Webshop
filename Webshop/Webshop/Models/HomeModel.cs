using System.Collections.Generic;

namespace Webshop.Models
{
    public class HomeModel
    {
        public List<AllProductsViewModel> AllProducts { get; set; }
        public List<News> News { get; set; }
    }
}
