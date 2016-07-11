using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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