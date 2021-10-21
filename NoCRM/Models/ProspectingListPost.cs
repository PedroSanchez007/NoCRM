using System.Collections.Generic;
using System.Linq;

namespace NoCRM.Models
{
    public class ProspectingListPost
    {
        public static readonly string[] ContentHeaders = new [] 
        {
            "Nom d'opportunité",
            "CapitaId",
            "Category",
            "Date",
            "Type",
            "Surface",
            "Rooms",
            "Energy Class",
            "Localisation Town",
            "Department",
            "District",
            "Index",
            "Cost",
            "Téléphone",
            "Parse Date",
            "Status"
        };
        
        public string title { get; set; }
        public string[][] content { get; set; } 

        /// <summary>
        /// Object required for ProspectingList updates. It does not contain the headers or a title as these already exist.
        /// </summary>
        /// <param name="capitaProspects"></param>
        public ProspectingListPost(List<CapitaProspect> capitaProspects)
        {
            var contentList = new List<string[]>();
            contentList.AddRange(capitaProspects.Select(p => p.AsNoCrmPostRecord()));
            content = contentList.ToArray();
        }
        
        /// <summary>
        /// Object required for ProspectingList creation. It contains the headers and a title.
        /// </summary>
        /// <param name="capitaProspects"></param>
        /// <param name="newListTitle"></param>
        public ProspectingListPost(List<CapitaProspect> capitaProspects, string newListTitle)
        {
            title = newListTitle;
            var contentList = new List<string[]> { ContentHeaders };
            contentList.AddRange(capitaProspects.Select(p => p.AsNoCrmPostRecord()));
            content = contentList.ToArray();
        }
    }
}