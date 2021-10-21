﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using Newtonsoft.Json;
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
            var queryString = GetQueryString(queryParameters);
            var endPointUrl = endPoint + queryString;
            return DoRequest(HttpMethod.Get, endPointUrl);
        }
        
        public string MakePostRequest(string endPoint, object postParameters)
        {
            var queryString = GetQueryString(queryParameters: null);
            // Maybe I should receive parameters and jsonify them here
            
            var endPointUrl = endPoint + queryString;
            return DoRequest(HttpMethod.Post, endPointUrl, postParameters);
        }
        public string MakePutRequest(string endPoint, object putParameters)
        {
            var queryString = GetQueryString(queryParameters: null);
            // Maybe I should receive parameters and jsonify them here
            
            var endPointUrl = endPoint + queryString;
            return DoRequest(HttpMethod.Put, endPointUrl, putParameters);
        }
        
        private string DoRequest(HttpMethod httpMethod, string endPoint, object parameters = default)
        {
            var json = JsonConvert.SerializeObject(parameters);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var httpTask = GetHttpTask(httpMethod, endPoint, content);
            var result = httpTask.GetAwaiter().GetResult();
            if (result.IsSuccessStatusCode)
                return result.Content.ReadAsStringAsync().Result;

            Log.Error($"Http Request to { _client.BaseAddress + endPoint} failed with error code { result.StatusCode }");
            if (httpMethod == HttpMethod.Put && result.StatusCode == HttpStatusCode.BadRequest)
                Log.Error($"The error occurs when the data sent to update the CRM does not match the number of fields in the table that is being updated. Although it could be something else.");
            return string.Empty;
        }

        private ConfiguredTaskAwaitable<HttpResponseMessage> GetHttpTask(HttpMethod httpMethod, string endpoint, HttpContent content = null)
        {
            if (content == null && new[] { "Post", "Put", "Patch" }.Contains(httpMethod.ToString()))
                throw new ArgumentException($"No content was passed to HTTP call { httpMethod }");

            return httpMethod.ToString() switch
            {
                "GET" => _client.GetAsync(endpoint).ConfigureAwait(false),
                "POST" => _client.PostAsync(endpoint, content).ConfigureAwait(false),
                "PUT" => _client.PutAsync(endpoint, content).ConfigureAwait(false),
                "PATCH" => _client.PatchAsync(endpoint, content).ConfigureAwait(false),
                "DELETE" => _client.DeleteAsync(endpoint).ConfigureAwait(false),
                _ => default
            };
        }

        private string GetQueryString(Dictionary<string, string> queryParameters = null)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (!string.IsNullOrEmpty(_apiKey.Key) && !string.IsNullOrEmpty(_apiKey.Value))
            {
                query[_apiKey.Key] = _apiKey.Value;
            }

            if (queryParameters != null)
            {
                foreach (var (key, value) in queryParameters)
                {
                    query[key] = value;
                }                
            }

            return query.HasKeys() ? "?" + query : string.Empty;
        }
    }
}