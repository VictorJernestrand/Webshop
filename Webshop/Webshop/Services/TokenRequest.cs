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

        public TokenRequest(IHttpClientFactory clientFactory)
        {
            webAPI = new WebAPIHandler(clientFactory);
        }

        /// <summary>
        /// Sends a request with an old token containing a refresh-token for getting a new valid JWT token from API
        /// </summary>
        /// <param name="cookieToken"></param>
        /// <returns></returns>
        public async Task<string> New(string cookieToken)
        {
            // Request a new token
            APIPayload auth = new APIPayload()
            {
                Token = cookieToken
            };

            var token = await webAPI.PostAsync<APIPayload>(auth, "https://localhost:44305/api/token/renew");
            return token.ResponseContent;
        }
    }
}
