using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Domain
{
    public class APIPayload
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
