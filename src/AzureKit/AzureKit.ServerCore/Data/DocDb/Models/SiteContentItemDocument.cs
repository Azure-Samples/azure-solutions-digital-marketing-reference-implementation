using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace AzureKit.Data.DocDb.Models
{
    public class SiteContentItemDocument : Document
    {
        public SiteContentItemDocument()
        {}
        public SiteContentItemDocument(AzureKit.Models.ContentType contentType)
        {
            this.ContentType = contentType.ToString();
        }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("contentType")]
        public string ContentType { get; set; }

        [JsonProperty("htmlContent")]
        public string HtmlContent { get; set; }

        [JsonProperty("availableOnMobile")]
        public bool AvailableOnMobileApps { get; set; }

    }
}