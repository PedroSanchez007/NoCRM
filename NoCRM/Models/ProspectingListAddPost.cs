using System.Collections.Generic;
using System.Linq;

namespace NoCRM.Models
{
    public class ProspectingListAddPost
    {
        public string[][] content { get; set; } 

        /// <summary>
        /// Object required for ProspectingList updates. It does not contain the headers or a title as these already exist.
        /// </summary>
        /// <param name="capitaProspects"></param>
        public ProspectingListAddPost(IEnumerable<CapitaProspect> capitaProspects)
        {
            content = capitaProspects.Select(p => p.AsNoCrmPostRecord()).ToArray();
        }
    }
}