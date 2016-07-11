using Newtonsoft.Json;

namespace AzureKit.Data.DocDb.Models
{
    public class SiteMapEntryDescription
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("contentId")]
        public string ContentIdentifier { get; set; }
    }
}