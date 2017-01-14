namespace AZKitMobile.Models
{
    /// <summary>
    /// Represents the content from the server.
    /// Content from the server is serialized and the properties need to 
    /// match. All server content has the commen properties.
    /// </summary>
    public class ContentModelBase
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

    }
}
