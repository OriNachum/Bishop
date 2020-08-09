using System.Net.Http;

namespace Bishop.Service
{
    public interface IHttpClientProvider
    {
        HttpClient GetHttpClient();
    }
}