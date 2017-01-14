using System.Collections.Generic;

namespace AzureKit.Models
{
    public class ListLandingContent : ContentModelBase
    {
        public ListLandingContent() :base(ContentType.ListLanding)
        {}

        public List<ListItemContent> Items { get; set; }
    }
}