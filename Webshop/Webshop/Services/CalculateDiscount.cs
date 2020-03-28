using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Services
{
    public class CalculateDiscount
    {
        public static decimal NewPrice(decimal price, decimal discount)
        {
            return price - (price * discount);
        }
    }
}
