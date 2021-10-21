using System.Linq;

namespace NoCRM.Models
{
    public class ProspectPost
    {
        public string[] content { get; set; } 

        public ProspectPost(CapitaProspect capitaProspect)
        {
            content = capitaProspect.AsNoCrmPostRecord().ToArray();
        }
    }
}