using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NoCRM.Models;

namespace NoCRM
{
    public static class CapitaCommunication
    {
        private const string WebDomain = @"https://capitalead.gndex.com/api/v2/";
        private static readonly KeyValuePair<string, string> ApiKey = new ("key", "b4e6dda1-806b-4627-8fbc-d1fdea846f42");
        private const string AllData = @"alldata";
        private const string NewData = @"newdata";

        public static IEnumerable<CapitaProspect> GetNewData()
        {
            return GetData(NewData);
        }
        
        public static IEnumerable<CapitaProspect> GetAllData()
        {
            return GetData(AllData);
        }

        private static IEnumerable<CapitaProspect> GetData(string endPoint)
        {
            var capitaCommunication = new HttpCommunication(WebDomain, ApiKey);
            var data = capitaCommunication.MakeGetRequest(endPoint);
            var newRecords = JsonConvert.DeserializeObject<List<CapitaProspect>>(data);
            var qualifyingNewRecords = newRecords?.Where(r => r.Category == "Rent" && r.Phone != null).Distinct(new RecordComparer());

            return qualifyingNewRecords;
        }
    }
}