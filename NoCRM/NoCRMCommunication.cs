using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using NoCRM.Models;

namespace NoCRM
{
    public static class NoCrmCommunication
    {
        private const string PingEndpoint = "ping";
        private const string ProspectingListsEndpoint = "spreadsheets";
        private const string LeadsEndpoint = "leads";
        
        //private const string NoCrmWebDomain = @"https://solutions-in-spades.nocrm.io/api/v2/"; // OtherCrmApiKey
        private const string NoCrmWebDomain = @"https://levy26.nocrm.io/api/v2/";

        private static KeyValuePair<string, string> ApiKey => new (Program.AppSettings.NoCrmApiKeyName, Program.AppSettings.NoCrmApiKey);

        public static bool Ping()
        {
            var httpCommunication = new HttpCommunication(NoCrmWebDomain, ApiKey);
            var response = httpCommunication.MakeGetRequest(PingEndpoint);
            return response != string.Empty;
        }
        public static string GetAllLeads()
        {
            var httpCommunication = new HttpCommunication(NoCrmWebDomain, ApiKey);
            return httpCommunication.MakeGetRequest(LeadsEndpoint);
        }
        
        public static IEnumerable<NoCrmProspect> GetAllProspects()
        {
            var prospectingListsWithoutProspects = GetProspectingLists();
            var prospectingListsWithRegionPrefix =
                prospectingListsWithoutProspects.Where(x => x.Title.Length >= 7 && x.Title.Substring(0, 7) == "region_");
            var prospectingLists = GetProspectingListsOfProspects(prospectingListsWithRegionPrefix.Select(l => l.Id).ToList());
            return prospectingLists.SelectMany(pl => pl.AsCrmProspects());
        }
        private static IEnumerable<ProspectingList> GetProspectingLists()
        {
            var httpCommunication = new HttpCommunication(NoCrmWebDomain, ApiKey);
            var data = httpCommunication.MakeGetRequest(ProspectingListsEndpoint);
            return JsonConvert.DeserializeObject<List<ProspectingList>>(data, new JsonSerializerSettings(){ Culture = new CultureInfo("en-GB") });
        }
        private static ProspectingList GetProspectingList(int prospectingListId)
        {
            var httpCommunication = new HttpCommunication(NoCrmWebDomain, ApiKey);
            var endpoint = $"{ProspectingListsEndpoint}/{prospectingListId}"; 
            var data = httpCommunication.MakeGetRequest(endpoint);
            return JsonConvert.DeserializeObject<ProspectingList>(data, new JsonSerializerSettings(){ Culture = new CultureInfo("en-GB") });
        }
        private static IEnumerable<ProspectingList> GetProspectingListsOfProspects(IEnumerable<int> prospectingListIds)
        {
            return prospectingListIds.Select(GetProspectingList).ToList();
        }
        public static string CreateProspectingList(List<CapitaProspect> capitaProspects, string newListTitle)
        {
            var httpCommunication = new HttpCommunication(NoCrmWebDomain, ApiKey);
            var prospectingListPost = new ProspectingListPost(capitaProspects, newListTitle);
            return httpCommunication.MakePostRequest(ProspectingListsEndpoint, prospectingListPost);
        }
        public static string AddProspectsToProspectingList(int prospectingListId, IEnumerable<CapitaProspect> capitaProspects)
        {
            var httpCommunication = new HttpCommunication(NoCrmWebDomain, ApiKey);
            var endpoint = $"{ProspectingListsEndpoint}/{prospectingListId}/rows"; 
            var prospectingListPost = new ProspectingListAddPost(capitaProspects);
            return httpCommunication.MakePostRequest(endpoint, prospectingListPost);
        }
        public static string UpdateProspect(int prospectingListId, int prospectId, CapitaProspect capitaProspect)
        {
            var httpCommunication = new HttpCommunication(NoCrmWebDomain, ApiKey);
            var endpoint = $"{ProspectingListsEndpoint}/{prospectingListId}/rows/{prospectId}"; 
            var prospectingListPost = new ProspectPost(capitaProspect);
            return httpCommunication.MakePutRequest(endpoint, prospectingListPost);
        }
    }
}