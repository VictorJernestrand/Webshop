using System.Collections.Generic;

namespace Webshop.Models
{
    public class ProductOrderViewModel
    {
        public List<AllUserOrders> Orders { get; set; }
        public List<Status> Statuses { get; set; }
    }
}
