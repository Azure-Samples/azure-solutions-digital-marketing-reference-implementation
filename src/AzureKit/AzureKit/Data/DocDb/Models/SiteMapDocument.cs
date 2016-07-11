using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace AzureKit.Data.DocDb.Models
{
    public class SiteMapDocument:Document
    {
        public SiteMapDocument()
        {
            DocumentType = "SiteMap";
        }

        [JsonProperty("documentType")]
        public string DocumentType { get; private set; }

        [JsonProperty("entries")]
        public SiteMapEntryDescription[] Entries { get; set; }

    }

   
}