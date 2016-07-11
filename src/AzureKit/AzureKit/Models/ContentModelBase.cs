using System.Web.Mvc;

namespace AzureKit.Models
{
    public class ContentModelBase
    {
        public ContentModelBase()
        {}
        public ContentModelBase(ContentType contentType)
        {
            this.ContentType = contentType;
        }
        public string Id { get; set; }

        public string Title { get; set; }

        public ContentType ContentType { get; set; }

        [AllowHtml]
        public string Content { get; set; }

        public MvcHtmlString Html { get; set; }

        public bool AvailableOnMobileApps { get; set; }

    }
}