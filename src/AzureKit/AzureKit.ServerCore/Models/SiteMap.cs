using System.Collections.Generic;

namespace AzureKit.Models
{
    public class SiteMap
    {
        public string Id { get; set; }

        public List<SiteMapEntry> Entries { get; set; }
    }
}