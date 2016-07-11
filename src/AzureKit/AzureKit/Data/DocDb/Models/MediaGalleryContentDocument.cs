using Newtonsoft.Json;

namespace AzureKit.Data.DocDb.Models
{
    public class MediaGalleryContentDocument : SiteContentItemDocument
    {
        public MediaGalleryContentDocument() : base(AzureKit.Models.ContentType.MediaGallery)
        {}
        
        [JsonProperty("items")]
        public MediaItem[] Items { get; set; }
    }
}