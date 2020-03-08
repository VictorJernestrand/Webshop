using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class CartButtonInfoModel
    {
        //public Guid CartId { get; set; }
        public int TotalItems { get; set; } = 0;
        public string TotalCost { get; set; } = "0";

    }
}
