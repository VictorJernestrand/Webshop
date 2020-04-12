using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebAPI.Domain;

namespace Webshop.Services
{
    public class WebAPIHandler
    {
        // Declare an HttpClient for connectivity to the WebAPI
        private readonly HttpClient _clientFactory;

        // Set headers and values
        const string ACCEPT_HEADER      = "Accept";
        const string USER_AGENT_HEADER  = "User-Agent";
        const string USER_AGENT_VALUE   = "WebshopProject";
        const string ACCEPT_VALUE       = "application/json";

        /// <summary>
        /// Instantiate a new WebAPIHandler with a IHttpClientFactory parameter.
        /// The IHttpClientFactory object is required for communicating with the API
        /// </summary>
        /// <param name="clientFactory"></param>
        public WebAPIHandler(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory.CreateClient();
        }

        /// <summary>
        /// Send a POST request of T to API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="webApiPath"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<APIResponseData> PostAsync<T>(T obj, string webApiPath, string token = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, webApiPath);

            if (token != null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Serialize object to JSON
            var postJson = JsonSerializer.Serialize(obj);
            request.Content = new StringContent(postJson, Encoding.UTF8, ACCEPT_VALUE);

            // Send and receive request
            var result = await SendRequestAsync(request);
            var responseString = await result.Content.ReadAsStringAsync();

            //var payload = await DeserializeJSON<APIPayload>(responseString);
            return new APIResponseData() { Status = result, ResponseContent = responseString, APIPayload = DeserializeTokens(responseString) };
        }

        /// <summary>
        /// Get a collection of T from API
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> GetAllAsync<T>(string webApiPath, string token = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, webApiPath);

            if (token != null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _clientFactory.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await DeserializeJSON<List<T>>(response);
            }

            return null;
        }

        /// <summary>
        /// Request a single object of T from API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webApiPath"></param>
        /// <returns></returns>
        public async Task<T> GetOneAsync<T>(string webApiPath, string token = null) where T : class
        {
            var request = new HttpRequestMessage(HttpMethod.Get, webApiPath);

            if (token != null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _clientFactory.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await DeserializeJSON<T>(response);
            }

            return null;
        }

        /// <summary>
        /// Send an UPDATE request of T to API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="webApiPath"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<APIResponseData> UpdateAsync<T>(T obj, string webApiPath, string token = null)
        {
            HttpResponseMessage response = null;
            using (var request = new HttpRequestMessage(HttpMethod.Put, webApiPath))
            {
                // Send JWT authentication token
                if(token != null)
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var serialized = JsonSerializer.Serialize(obj);
                request.Content = new StringContent(serialized, Encoding.UTF8, ACCEPT_VALUE);

                response = await _clientFactory.SendAsync(request);

                var responseString = await response.Content.ReadAsStringAsync();

                return new APIResponseData() { Status = response, ResponseContent = responseString, APIPayload = DeserializeTokens(responseString) };
            }

        }

        /// <summary>
        /// Send a DELETE request of T to API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webApiPath"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(string webApiPath, string token = null)
        {
            HttpResponseMessage response = null;
            using (var request = new HttpRequestMessage(HttpMethod.Delete, webApiPath))
            {
                if (token != null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                response = await _clientFactory.SendAsync(request);
            }

            return (response.IsSuccessStatusCode) ? true : false;
        }

        /// <summary>
        /// Check if T exist in API
        /// </summary>
        /// <param name="webApiPath"></param>
        /// <returns></returns>
        public async Task<bool> ExistAsync(string webApiPath)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, webApiPath);

            var response = await _clientFactory.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                    return await JsonSerializer.DeserializeAsync<bool>(responseStream);
            }

            // throw an error
            throw new HttpRequestException();
        }

        /// <summary>
        /// Send request message to API
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request)
        {
            request = SetHeaders(request);
            return await _clientFactory.SendAsync(request);
        }

        /// <summary>
        /// Set default request headers
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private HttpRequestMessage SetHeaders(HttpRequestMessage request)
        {
            request.Headers.Add(ACCEPT_HEADER, ACCEPT_VALUE);
            request.Headers.Add(USER_AGENT_HEADER, USER_AGENT_VALUE);
            return request;
        }

        /// <summary>
        /// Deserialize JSON back to object of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        public async Task<T> DeserializeJSON<T>(HttpResponseMessage response)
        {
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var post = await JsonSerializer.DeserializeAsync<T>(responseStream,
                    new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    }
                );
                
                return post;
            }
        }

        public T DeserializeJSON<T>(string jsonString)
        {
            var post = JsonSerializer.Deserialize<T>(jsonString,
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                }
            );

            return post;
        }


        private APIPayload DeserializeTokens(string tokenJson)
        {
            try
            {
                var post = JsonSerializer.Deserialize<APIPayload>(tokenJson, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });

               return post;
            }
            catch (Exception)
            {
                return new APIPayload();
            }
        }

    }
}
