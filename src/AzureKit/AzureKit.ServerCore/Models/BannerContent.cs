using AzureKit.Config;
using System;
using System.ComponentModel.DataAnnotations;

namespace AzureKit.Models
{
    public class BannerContent : ContentModelBase
    {
        public BannerContent():base(ContentType.Banner)
        {
            base.Id = Constants.KEY_BANNER_CONTENT;
        }

        [Required]
        public DateTime Expiration { get; set; }
    }
}