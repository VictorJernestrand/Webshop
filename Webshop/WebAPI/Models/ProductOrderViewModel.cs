using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models.Data;

namespace WebAPI.Models
{
    public class ProductOrderViewModel
    {
       // public int productOrderId { get; set; }
        public int orderId { get; set; }
        public DateTime orderCreationDate { get; set; }
        public int productId { get; set; }
        public decimal productCost { get; set; }
        public int Quantity { get; set; }
        public decimal price { get; set; }
        public List<Status> statuslist { get; set; }
        public string orderstatus { set; get; }
        public Status status { get; set; }
        public int statusId { get; set; }
       

    }
}
