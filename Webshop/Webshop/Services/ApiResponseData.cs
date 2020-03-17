using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Webshop.Services
{
    public class APIResponseData
    {
        public HttpResponseMessage Status { get; set; }
        public string ResponseContent { get; set; }
    }
}
