using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Webshop.Domain;

namespace Webshop.Services
{
    public class WebAPIToken
    {
        private readonly WebAPIHandler webAPI;
        private readonly IHttpContextAccessor accessor;
        //private readonly IHttpClientFactory clientFactory;
        //private readonly IConfiguration config;

        private readonly string _jwtTokenCookie;
        private readonly string _refreshTokenCookie;
        private readonly int _expireMonths;

        /// <summary>
        /// Request a new JWT-token from WebAPI by providing old JWT-token stored in local token cookie
        /// </summary>
        /// <param name="webAPI"></param>
        /// <param name="accessor"></param>
        /// <param name="clientFactory"></param>
        /// <param name="config"></param>
        public WebAPIToken(WebAPIHandler webAPI, IHttpContextAccessor accessor, IHttpClientFactory clientFactory, IConfiguration config)
        {
            this.webAPI = webAPI;
            this.accessor = accessor;

            this._jwtTokenCookie = config["JwtSessionToken:Name"];
            this._refreshTokenCookie = config["RefreshToken:Name"];
            this._expireMonths = int.Parse(config["RefreshToken:Expire"]);
        }

        /// <summary>
        /// Sends a request with an old token containing a refresh-token for getting a new valid JWT token from API
        /// </summary>
        /// <param name="cookieToken"></param>
        /// <returns></returns>
        public async Task<string> New()
        {
            APIResponseData response = new APIResponseData();

            if (SessionTokenRefresh == null)
            {
                APIPayload payload = new APIPayload()
                {
                    RefreshToken = TokenRefreshCookie,
                    UserEmail = accessor.HttpContext.User.Identity.Name
                };

                response = await webAPI.PostAsync(payload, "https://localhost:44305/api/tokenrequest/refresh");
            }
            else
            {

                // Instantiate a new payload-object containing old JWT (Jason Web Token) from local cookie 'TastyCookie'
                APIPayload payload = new APIPayload()
                {
                    //Token = TokenRefreshCookie // Get old JWT token from cookie
                    Token = SessionTokenRefresh,
                    RefreshToken = TokenRefreshCookie
                };

                // Post payload object to API and request a new token.
                // The API will validate the old JWT-token and return a new token if old JWT wasn't tampered with.
                response = await webAPI.PostAsync(payload, "https://localhost:44305/api/tokenrequest/new");
            }

            // Invalid token
            if (!response.Status.IsSuccessStatusCode)
            {
                // Send request using refresh token
            }

            //if (string.IsNullOrEmpty(token.ResponseContent))
            //{
            //    throw new ArgumentNullException("Token was null!");
            //}

            // Store new token in local cookie
            //TokenRefreshCookie = newToken.ResponseContent;
            SessionTokenRefresh = response.APIPayload.Token;


            //var test = accessor.HttpContext.Session.GetString("TestCookie");
            return response.APIPayload.Token;
        }

        /// <summary>
        /// Property for setting and getting JWT-token from cookie
        /// </summary>
        public string TokenRefreshCookie
        {
            get { return accessor.HttpContext.Request.Cookies[_refreshTokenCookie]; }
            set { accessor.HttpContext.Response.Cookies.Append(_refreshTokenCookie, value, Options(_expireMonths)); }
        }

        public string SessionTokenRefresh
        {
            get { return accessor.HttpContext.Session.GetString(_jwtTokenCookie); }
            set
            {
                accessor.HttpContext.Session.SetString(_jwtTokenCookie, value);
                //accessor.HttpContext.Response.Cookies.Append(_tokenCookie, value, Options(_expireMonths));
            }
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
