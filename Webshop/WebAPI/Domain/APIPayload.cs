using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Domain
{
    public class APIPayload
    {
        // Cointains the actual JWT token
        public string Token { get; set; }

        // Containts the RefreshToken stored in the Users table in the database
        public string RefreshToken { get; set; }

        public string UserEmail { get; set; }
    }
}
