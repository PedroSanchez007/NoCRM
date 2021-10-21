using System.Collections.Generic;
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
        
        //private const string NoCrmWebDomain = @"https://solutions-in-spades.nocrm.io/api/v2/";
        private const string NoCrmWebDomain = @"https://levy26.nocrm.io/api/v2/";
        // private static readonly KeyValuePair<string, string> NoCrmApiKey = new ("api_key", "d798703ed9d2dae9c27233445770e3e66bd2043ae1eb512d");
        private static readonly KeyValuePair<string, string> NoCrmApiKey = new ("api_key", "f414157dc83719c8773ce4d6c88101be4b84f66a8ec25e20");

        public static bool Ping()
        {
            var httpCommunication = new HttpCommunication(NoCrmWebDomain, NoCrmApiKey);
            var response = httpCommunication.MakeGetRequest(PingEndpoint);
            return response != string.Empty;
        }
        public static string GetAllLeads()
        {
            var httpCommunication = new HttpCommunication(NoCrmWebDomain, NoCrmApiKey);
            return httpCommunication.MakeGetRequest(LeadsEndpoint);
        }
        
        public static IEnumerable<NoCrmProspect> GetAllProspects()
        {
            var prospectingListsWithoutProspects = GetProspectingLists();
            var prospectingLists = GetProspectingListsOfProspects(prospectingListsWithoutProspects.Select(l => l.Id).ToList());
            return prospectingLists.SelectMany(pl => pl.AsCrmProspects());
        }
        private static IEnumerable<ProspectingList> GetProspectingLists()
        {
            var httpCommunication = new HttpCommunication(NoCrmWebDomain, NoCrmApiKey);
            var data = httpCommunication.MakeGetRequest(ProspectingListsEndpoint);
            return JsonConvert.DeserializeObject<List<ProspectingList>>(data);
        }
        private static ProspectingList GetProspectingList(int prospectingListId)
        {
            var httpCommunication = new HttpCommunication(NoCrmWebDomain, NoCrmApiKey);
            var endpoint = $"{ProspectingListsEndpoint}/{prospectingListId}"; 
            var data = httpCommunication.MakeGetRequest(endpoint);
            return JsonConvert.DeserializeObject<ProspectingList>(data);
        }
        private static IEnumerable<ProspectingList> GetProspectingListsOfProspects(IEnumerable<int> prospectingListIds)
        {
            return prospectingListIds.Select(GetProspectingList).ToList();
        }
        public static string CreateProspectingList(List<CapitaProspect> capitaProspects, string newListTitle)
        {
            var httpCommunication = new HttpCommunication(NoCrmWebDomain, NoCrmApiKey);
            var prospectingListPost = new ProspectingListPost(capitaProspects, newListTitle);
            return httpCommunication.MakePostRequest(ProspectingListsEndpoint, prospectingListPost);
        }
        public static string AddProspectsToProspectingList(int prospectingListId, List<CapitaProspect> capitaProspects)
        {
            var httpCommunication = new HttpCommunication(NoCrmWebDomain, NoCrmApiKey);
            var endpoint = $"{ProspectingListsEndpoint}/{prospectingListId}/rows"; 
            var prospectingListPost = new ProspectingListPost(capitaProspects);
            return httpCommunication.MakePostRequest(endpoint, prospectingListPost);
        }
        public static string UpdateProspect(int prospectingListId, int prospectId, CapitaProspect capitaProspect)
        {
            var httpCommunication = new HttpCommunication(NoCrmWebDomain, NoCrmApiKey);
            var endpoint = $"{ProspectingListsEndpoint}/{prospectingListId}/rows/{prospectId}"; 
            var prospectingListPost = new ProspectPost(capitaProspect);
            return httpCommunication.MakePutRequest(endpoint, prospectingListPost);
        }
    }
}