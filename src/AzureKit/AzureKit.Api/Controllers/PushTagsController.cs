using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.NotificationHubs;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace AzureKit.Controllers.Mobile
{
    /// <summary>
    /// Controllers invoked from client applications to update the tags 
    /// associated with their notification subscription
    /// </summary>
    [MobileAppController]
    public class PushTagsController : ApiController
    {
        private NotificationHubClient _hubClient;

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            // Get the Mobile App settings.
            MobileAppSettingsDictionary settings =
                this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

            // Get the Notification Hubs credentials for the Mobile App.
            string notificationHubName = settings.NotificationHubName;
            string notificationHubConnection = settings
                .Connections[MobileAppSettingsKeys.NotificationHubConnectionString]
                .ConnectionString;

            // Create the notification hub client.
            _hubClient = NotificationHubClient
                .CreateClientFromConnectionString(notificationHubConnection,
                    notificationHubName);
        }

        [HttpPut]
        public async Task<HttpResponseMessage> UpdateTagsForInstallation(string Id)
        {
            // Get the tags to update from the body of the request.
            var message = await this.Request.Content.ReadAsStringAsync();

            // Validate the submitted tags do not try to overwrite tags created for user.
            if (string.IsNullOrEmpty(message) || message.Contains("sid:"))
            {
                // We can't trust users to submit their own user IDs.
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            // Verify that the tags are a valid JSON array.
            var tags = JArray.Parse(message);

            // Define a collection of PartialUpdateOperations. Note that 
            // only one '/tags' path is permitted in a given collection.
            var updates = new List<PartialUpdateOperation>();

            // Add a replace operation for the tag.
            updates.Add(new PartialUpdateOperation
            {
                Operation = UpdateOperationType.Replace,
                Path = "/tags",
                Value = tags.ToString()
            });
    
            // Add the requested tags to the installation.
            await _hubClient.PatchInstallationAsync(Id, updates);

            // Return success status.
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
