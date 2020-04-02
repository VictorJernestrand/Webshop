using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class OrderAndPaymentMethods
    {
        public User User { get; set; }


        // Payment methods
        public OrderViewModel OrderViewModel { get; set; }

        public CreditCardModel CreditCardModel { get; set; }

        // Add more paymentmethod-modells here here...
    }
}
