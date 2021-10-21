using System;
using System.Collections.Generic;
using System.Linq;

namespace NoCRM.Models
{
    public class ProspectingList
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Column_names { get; set; }
        public int Privacy { get; set; }
        public DateTime Updated_at { get; set; }
        public DateTime Created_at { get; set; }
        public string[] Tags { get; set; }
        public string Permalink { get; set; }
        public ProspectDump[] Spreadsheet_rows { get; set; }

        public List<NoCrmProspect> AsCrmProspects()
        {
            return Spreadsheet_rows.Select(p => new NoCrmProspect(Id, Title, p.Id, p.Content)).ToList();
        }
    }
}