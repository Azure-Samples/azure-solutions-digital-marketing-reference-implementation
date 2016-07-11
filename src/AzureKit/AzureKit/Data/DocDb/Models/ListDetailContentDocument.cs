using Newtonsoft.Json;

namespace AzureKit.Data.DocDb.Models
{
    public class ListDetailContentDocument : SiteContentItemDocument
    {
        public ListDetailContentDocument() : base(AzureKit.Models.ContentType.ListItem)
        {}


        [JsonProperty("listLandingId")]
        public string ListLandingId { get; set; }

        [JsonProperty("landingPageSummary")]
        public string LandingPageSummary { get; set; }

    }
}