using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace AzureKit.Areas.Manage.Controllers
{
    public class NotificationsController : Controller
    {
        // GET: Manage/Notifications
        public ActionResult Index()
        {
            return View();
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(Models.SendNotificationsModel model)
        {
            //get notification hub information
            // Get the settings for the server project.

            System.Web.Http.HttpConfiguration config = 
                System.Web.Http.GlobalConfiguration.Configuration;
            MobileAppSettingsDictionary settings =
                config.GetMobileAppSettingsProvider().GetMobileAppSettings();

            // Get the Notification Hubs credentials for the Mobile App.
            string notificationHubName = settings.NotificationHubName;
            string notificationHubConnection = settings
                .Connections[MobileAppSettingsKeys.NotificationHubConnectionString].ConnectionString;

            // Create a new Notification Hub client.
            NotificationHubClient hub = NotificationHubClient
            .CreateClientFromConnectionString(notificationHubConnection, notificationHubName);

            // Sending the message so that all template registrations that contain "messageParam"
            // will receive the notifications. This includes APNS, GCM, WNS, and MPNS template registrations.
            Dictionary<string, string> templateParams = new Dictionary<string, string>();
            templateParams["title"] = model.Title;
            templateParams["message"] = model.Message;

            try
            {
                NotificationOutcome result = null;

                // Send the push notification and log the results.
                if (model.Tags != null && model.Tags.Count > 0)
                {
                     result = await hub.SendTemplateNotificationAsync(templateParams, String.Join(" || ", model.Tags));
                }
                else
                {
                    result = await hub.SendTemplateNotificationAsync(templateParams);
                }

                // Write the success result to the logs.
                config.Services.GetTraceWriter().Info(result.State.ToString());
            }
            catch (System.Exception ex)
            {
                // Write the failure result to the logs.
                config.Services.GetTraceWriter()
                    .Error(ex.Message, null, "Push.SendAsync Error");
                throw;
            }

            //redirct to confirm
            return View("Confirm");

        }
    }
}