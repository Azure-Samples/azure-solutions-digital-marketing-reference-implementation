using System.ComponentModel.DataAnnotations;

namespace AzureKit.Models
{
    public class ListItemContent :ContentModelBase
    {
        public ListItemContent():base(ContentType.ListItem)
        {}

        [Required]
        public string ListLandingId { get; set; }

        [Required]
        public string LandingPageSummary { get; set; }
    }
}