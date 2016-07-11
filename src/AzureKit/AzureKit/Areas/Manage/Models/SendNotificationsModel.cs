using System.Collections.Generic;

namespace AzureKit.Areas.Manage.Models
{
    /// <summary>
    /// View model for sending notifications to users
    /// </summary>
    public class SendNotificationsModel
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public List<string> Tags { get; set; }

    }
}