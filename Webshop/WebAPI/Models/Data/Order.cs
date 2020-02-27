using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsAPI.Models
{
    public class Order
    {
        public Order()
        {
            this.OrderDate = DateTime.Now;
        }
        public int Id { get; set; }
        //public int UserId { get; set; }
        public int PaymentMethodId { get; set; }
        public DateTime OrderDate { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public User User { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

    }
}
