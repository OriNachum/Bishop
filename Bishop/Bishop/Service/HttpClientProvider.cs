using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Bishop.Service
{
    public class HttpClientProvider : IHttpClientProvider
    {
        private readonly HttpClient _httpClient;

        public HttpClientProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public HttpClient GetHttpClient()
        {
            return _httpClient;
        }
    }
}
