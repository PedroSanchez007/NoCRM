using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace NoCRM
{
    public class HttpRequestInfo
    {
        public HttpMethod HttpMethod { get; }
        public string EndPoint { get; }
        public Dictionary<string, string> QueryParameters { get; }
        public object ContentParameters { get; }
        public KeyValuePair<string, string> ApiKey { get; }
        public string QueryString => GetQueryString();
        public string SerializedParameters => JsonConvert.SerializeObject(ContentParameters, new JsonSerializerSettings(){ Culture = new CultureInfo("en-GB") });
        public StringContent Content => new (SerializedParameters, Encoding.UTF8, "application/json");

        public HttpRequestInfo(HttpMethod httpMethod, string endPoint, object contentParameters, KeyValuePair<string, string> apiKey)
        {
            HttpMethod = httpMethod;
            EndPoint = endPoint;
            ContentParameters = contentParameters;
            ApiKey = apiKey;
        }
        
        public HttpRequestInfo(HttpMethod httpMethod, string endPoint, KeyValuePair<string, string> apiKey, Dictionary<string, string> queryParameters)
        {
            HttpMethod = httpMethod;
            EndPoint = endPoint;
            QueryParameters = queryParameters;
            ApiKey = apiKey;
        }
        
        private string GetQueryString()
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (!string.IsNullOrEmpty(ApiKey.Key) && !string.IsNullOrEmpty(ApiKey.Value))
            {
                query[ApiKey.Key] = ApiKey.Value;
            }

            if (QueryParameters != null)
            {
                foreach (var (key, value) in QueryParameters)
                {
                    query[key] = value;
                }                
            }

            return EndPoint + (query.HasKeys() ? "?" + query : string.Empty);
        }

        public override string ToString()
        {
            return $"Method: {HttpMethod}, Endpoint: {EndPoint}, Query Parameters: {QueryParameters}, Content Parameters: {ContentParameters}";
        }
    }
}