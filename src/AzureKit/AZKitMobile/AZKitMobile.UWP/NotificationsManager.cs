using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using Windows.Networking.PushNotifications;

//Register dependency with Xamarin dependency service
[assembly: Xamarin.Forms.Dependency(typeof(AZKitMobile.UWP.NotificationsManager))]
namespace AZKitMobile.UWP
{
    public class NotificationsManager : AZKitMobile.INotificationsManager
    {
        /// <summary>
        /// Registers with the mobile app backend for push notifications
        /// </summary>
        /// <param name="client">The client to use for registration</param>
        /// <returns></returns>
        public async Task RegisterForPushNotifications(MobileServiceClient client)
        {
            var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
            if (channel != null && (!String.IsNullOrEmpty(channel.Uri)))
            {
                var templates = CreateTemplatePayload();
                await client.GetPush().RegisterAsync(channel.Uri, templates);
            }    
        }

        /// <summary>
        /// Createst the template object used to define alert/push message format.
        /// </summary>
        /// <returns></returns>
        private JObject CreateTemplatePayload()
        {
            JObject templateBody = new JObject();
            templateBody["body"] = Constants.TEMPLATE_UWP;

            // Add the required WNS toast header.
            JObject wnsToastHeaders = new JObject();
            wnsToastHeaders["X-WNS-Type"] = "wns/toast";
            templateBody["headers"] = wnsToastHeaders;

            JObject templates = new JObject();
            templates["updatesTemplate"] = templateBody;

            return templates;

        }

        /// <summary>
        /// Lets the application know that platform registration has completed.
        /// </summary>
        public void NotifyReadyForNotifications()
        {
            Xamarin.Forms.MessagingCenter.Send<INotificationsManager>(this, Constants.KEY_MESSAGING_NOTIFICATIONS);
        }
    }
}
