using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Webshop.Domain;

namespace Webshop.Services
{
    public class TokenRequest
    {
        private readonly WebAPIHandler webAPI;
        private readonly IHttpContextAccessor accessor;
        private readonly IHttpClientFactory clientFactory;

        public TokenRequest(IHttpContextAccessor accessor, IHttpClientFactory clientFactory)
        {
            this.accessor = accessor;
            this.clientFactory = clientFactory;
            this.webAPI = new WebAPIHandler(clientFactory);
        }

        /// <summary>
        /// Sends a request with an old token containing a refresh-token for getting a new valid JWT token from API
        /// </summary>
        /// <param name="cookieToken"></param>
        /// <returns></returns>
        public async Task<string> New()
        {
            // Instantiate a new payload-object containing old JWT (Jason Web Token) from local cookie 'TastyCookie'
            APIPayload payload = new APIPayload()
            {
                Token = TokenRefreshCookie // Get old JWT token from cookie
            };

            // Post payload object to API and request a new toke
            // The API will validate the old JWT-token and return a new token if old JWT wasn't tampered with.
            var token = await webAPI.PostAsync<APIPayload>(payload, "https://localhost:44305/api/token/renew");

            // Store new token in cookie
            TokenRefreshCookie = token.ResponseContent;

            return token.ResponseContent;
        }

        /// <summary>
        /// Property for setting and getting JWT-token from cookie
        /// </summary>
        public string TokenRefreshCookie
        {
            get { return accessor.HttpContext.Request.Cookies["TastyCookie"]; }
            set { accessor.HttpContext.Response.Cookies.Append("TastyCookie", value, Options(6)); }
        }

        /// <summary>
        /// Predefined security options for cookies. Parameter value describes expiration in months from current date
        /// </summary>
        /// <param name="expire"></param>
        /// <returns></returns>
        private CookieOptions Options(int expire)
        {
            var options = new CookieOptions();
            options.Expires = DateTime.UtcNow.AddMonths(expire);    // Expiration date in months. Value comes from appsettings.json
            options.Secure = true;                                  // Keep cookie secure. Only send cookie content over HTTPS
            options.HttpOnly = true;                                // Protects the cookie from being accessed by javascript
            options.SameSite = SameSiteMode.Lax;                    // Enables only first-party cookies to be sent/accessed
            return options;
        }
    }
}
