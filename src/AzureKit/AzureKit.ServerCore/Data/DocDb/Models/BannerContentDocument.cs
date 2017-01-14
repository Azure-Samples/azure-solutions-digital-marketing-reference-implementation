using Newtonsoft.Json;
using System;

namespace AzureKit.Data.DocDb.Models
{
    public class BannerContentDocument : SiteContentItemDocument
    {
        public BannerContentDocument() : base(AzureKit.Models.ContentType.Banner)
        { }

        [JsonProperty("expiration")]
        public DateTime Expiration { get; set; }

    }
}