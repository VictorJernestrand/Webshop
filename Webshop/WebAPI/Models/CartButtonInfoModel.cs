using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class CartButtonInfoModel
    {
        public int TotalItems { get; set; } = 0;
        public string TotalCost { get; set; } = "0 kr";

    }
}
