using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
        /// Instantiate IHttpClientFactory for communicating with the WebAPI
        /// </summary>
        /// <param name="clientFactory"></param>
        public WebAPIHandler(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory.CreateClient();
        }

        /// <summary>
        /// Post data to WebAPI
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<bool> PostToWebAPIAsync<T>(T obj, string webApiPath)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, webApiPath);
            var postJson = JsonSerializer.Serialize(obj);
            request.Content = new StringContent(postJson, Encoding.UTF8, ACCEPT_VALUE);

            // Send request
            var result = await SendRequestAsync(request);
            return result.IsSuccessStatusCode;
        }

        /// <summary>
        /// Get a collection of IEnumerable type of T from WebAPI
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAllFromWebAPIAsync<T>(string webApiPath)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, webApiPath);

            var response = await _clientFactory.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    var posts = await JsonSerializer.DeserializeAsync<List<T>>(responseStream,
                        new JsonSerializerOptions()
                        {
                            PropertyNameCaseInsensitive = true
                        }
                    );

                    return posts;
                }
            }

            return null;
        }

        /// <summary>
        /// Get single object of T from WebAPI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webApiPath"></param>
        /// <returns></returns>
        public async Task<T> GetSingleFromWebAPIAsync<T>(string webApiPath) where T : class
        {
            var request = new HttpRequestMessage(HttpMethod.Get, webApiPath);
            var response = await _clientFactory.SendAsync(request);

            if (response.IsSuccessStatusCode)
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

            return null;
        }

        public async Task<bool> UpdateObjectInWebAPIAsync<T>(T obj, string webApiPath)
        {
            HttpResponseMessage response = null;
            using (var request = new HttpRequestMessage(HttpMethod.Put, webApiPath))
            {
                var serialized = JsonSerializer.Serialize(obj);
                request.Content = new StringContent(serialized, Encoding.UTF8, ACCEPT_VALUE);

                response = await _clientFactory.SendAsync(request);
            }

            return (response.IsSuccessStatusCode) ? true : false;
        }






        /// <summary>
        /// 
        /// </summary>
        /// <param name="webApiPath"></param>
        /// <returns></returns>
        public async Task<bool> ExistInWebAPIAsync(string webApiPath)
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
        /// Send request to WebApi async
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request)
        {
            request = SetHeaders(request);
            return await _clientFactory.SendAsync(request);
        }

        // Set request headers based on the header constants
        private HttpRequestMessage SetHeaders(HttpRequestMessage request)
        {
            request.Headers.Add(ACCEPT_HEADER, ACCEPT_VALUE);
            request.Headers.Add(USER_AGENT_HEADER, USER_AGENT_VALUE);
            return request;
        }
    }
}
