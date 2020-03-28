﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models.Data;

namespace Webshop.Models
{
    public class OrderViewModel
    {
      
        public int Id { get; set; }
        //public int StatusId { get; set; }

        [Required(ErrorMessage = "Välj Payment Method")]
        public int PaymentMethodId { get; set; }

        //public int UserId { get; set; }
        public User User { get; set; }

        public bool AddressComplete { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public List<OrderItemsModel> Products { get; set; }

        public List<PaymentMethod> paymentMethodlist { get; set; } = new List<PaymentMethod>();

        public bool OrderContainsProductsOutOfStock { get; set; }
        public decimal OrderTotal { get; set; }



    }
}
