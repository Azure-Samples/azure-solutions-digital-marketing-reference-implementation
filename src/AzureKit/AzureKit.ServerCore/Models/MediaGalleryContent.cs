using System.Collections.Generic;

namespace AzureKit.Models
{
    public class MediaGalleryContent : ContentModelBase
    {
        public MediaGalleryContent() : base(ContentType.MediaGallery)
        {}

        public string BaseUrl { get; set; }

        public List<MediaItemModel> Items { get; set; }

    }
}