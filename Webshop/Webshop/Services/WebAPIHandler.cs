using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Webshop.Services
{
    public class WebAPIHandler<T>
    {
        private readonly string _path;
        private readonly HttpClient _client;

        public WebAPIHandler(IHttpClientFactory clientFactory, string path)
        {
            _path = path;
            _client = clientFactory.CreateClient();
        }

        public async Task<bool> PostToWebAPIAsync(T obj)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _path);
            request.Headers.Add("User-Agent", "WebshopProject");

            var postJson = JsonSerializer.Serialize(obj);
            request.Content = new StringContent(postJson, Encoding.UTF8, "application/json");

            var result = await SendRequestAsync(request);
            return result.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<T>> GetAllFromWebAPIAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _path);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", "WebshopProject");

            var response = await _client.SendAsync(request);

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

        public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request)
            => await _client.SendAsync(request);
    }
}
