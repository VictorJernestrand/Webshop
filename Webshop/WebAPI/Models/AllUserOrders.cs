using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class AllUserOrders
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public string OrderPayment { get; set; }
        public int StatusId { get; set; }

    }
}
