using System;
using System.Net.Http;

namespace NoCRM
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string baseAddress)
        {
            var client = new HttpClient();
            SetupClientDefaults(client, baseAddress);
            return client;
        }

        protected virtual void SetupClientDefaults(HttpClient client, string baseAddress)
        {
            client.Timeout = TimeSpan.FromSeconds(300); //set your own timeout.
            client.BaseAddress = new Uri(baseAddress);
        }
    }
    
    public interface IHttpClientFactory
    {
        HttpClient CreateClient(string baseAddress);
    }
}