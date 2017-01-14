namespace AzureKit.Models
{
    public class MediaItemModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ThumbnailUrl { get; set; }

        public string MediaUrl { get; set; }

        public string[] Tags { get; set; }
    }
}