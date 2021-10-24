using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Serilog;

namespace NoCRM
{
    public class HttpCommunication
    {
        // This needs to be made thread safe or static
        private readonly HttpClient _client;
        private readonly KeyValuePair<string, string> _apiKey;

        public HttpCommunication(string webDomain, KeyValuePair<string, string> apiKey)
        {
            _client = new HttpClient();
            _apiKey = apiKey;
            _client.BaseAddress = new Uri(webDomain);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        
        public string MakeGetRequest(string endPoint, Dictionary<string, string> queryParameters = null)
        {
            var httpRequestInfo = new HttpRequestInfo(HttpMethod.Get, endPoint, _apiKey, queryParameters);
            return DoRequest(httpRequestInfo);
        }
        
        public string MakePostRequest(string endPoint, object postParameters)
        {
            var httpRequestInfo = new HttpRequestInfo(HttpMethod.Post, endPoint, postParameters, _apiKey);
            return DoRequest(httpRequestInfo);
        }
        public string MakePutRequest(string endPoint, object putParameters)
        {
            var httpRequestInfo = new HttpRequestInfo(HttpMethod.Put, endPoint, putParameters, _apiKey);
            return DoRequest(httpRequestInfo);
        }
        
        private string DoRequest(HttpRequestInfo requestInfo)
        {
            var httpTask = GetHttpTask(requestInfo);
            var result = httpTask.GetAwaiter().GetResult();

            // A one second delay is necessary for NoCRM not to groan. Speed is not really an issue as this is a daily process.
            System.Threading.Thread.Sleep(1000);
            
            if (result.IsSuccessStatusCode)
                return result.Content.ReadAsStringAsync().Result;

            if (ErrorHandling(requestInfo, result))
                Environment.Exit((int)result.StatusCode);
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <param name="response"></param>
        /// <returns>Whether the error is fatal</returns>
        private bool ErrorHandling(HttpRequestInfo requestInfo, HttpResponseMessage response)
        {
            Log.Error($"Http Request to { _client.BaseAddress } with parameters { requestInfo } failed with error code { response.StatusCode }");

            if (requestInfo.HttpMethod == HttpMethod.Put && response.StatusCode == HttpStatusCode.BadRequest)
            {
                Log.Error($"The error occurs when the data sent to update the CRM does not match the number of fields in the table that is being updated. Although it could be something else.");
                return true;
            }

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                Log.Error($"Too many HTTP requests were made to NoCRM. As at 20-10-2021, the limit is 2000. Application is shutting down.");
                return true;
            }

            return false;
        }

        private ConfiguredTaskAwaitable<HttpResponseMessage> GetHttpTask(HttpRequestInfo requestInfo)
        {
            return requestInfo.HttpMethod.ToString() switch
            {
                "GET" => _client.GetAsync(requestInfo.QueryString).ConfigureAwait(false),
                "POST" => _client.PostAsync(requestInfo.QueryString, requestInfo.Content).ConfigureAwait(false),
                "PUT" => _client.PutAsync(requestInfo.QueryString, requestInfo.Content).ConfigureAwait(false),
                "PATCH" => _client.PatchAsync(requestInfo.QueryString, requestInfo.Content).ConfigureAwait(false),
                "DELETE" => _client.DeleteAsync(requestInfo.QueryString).ConfigureAwait(false),
                _ => default
            };
        }
    }
}