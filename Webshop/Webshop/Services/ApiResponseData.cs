using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebAPI.Domain;

namespace Webshop.Services
{
    public class APIResponseData
    {
        public HttpResponseMessage Status { get; set; }
        public string ResponseContent { get; set; }
        public APIPayload APIPayload { get; set; }
    }
}
