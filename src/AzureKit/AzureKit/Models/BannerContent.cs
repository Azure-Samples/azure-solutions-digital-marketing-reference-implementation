using AzureKit.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureKit.Models
{
    public class BannerContent : ContentModelBase
    {
        public BannerContent():base(ContentType.Banner)
        {
            base.Id = Constants.KEY_BANNER_CONTENT;
        }

        public DateTime Expiration { get; set; }
    }
}