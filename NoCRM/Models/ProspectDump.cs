namespace NoCRM.Models
{
    public class ProspectDump
    {
        public int Id { get; set; }
        public int? Lead_id { get; set; }
        public string[] Content { get; set; }
        public int Comment_count { get; set; }
    }
}