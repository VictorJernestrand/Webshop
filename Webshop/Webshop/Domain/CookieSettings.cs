using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Domain
{
    public class CookieSettings
    {
        /// <summary>
        /// Predefined security options for cookies. Parameter value describes expiration in months from current date
        /// </summary>
        /// <param name="expire"></param>
        /// <returns></returns>
        public static CookieOptions Set(int expire)
        {
            var options = new CookieOptions();
            options.Expires = DateTime.UtcNow.AddMonths(expire);
            options.Secure = true;
            options.SameSite = SameSiteMode.Lax;
            return options;
        }
    }
}
