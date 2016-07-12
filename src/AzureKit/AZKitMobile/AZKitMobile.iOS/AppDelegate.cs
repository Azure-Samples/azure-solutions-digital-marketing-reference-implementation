using Foundation;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using UIKit;

namespace AZKitMobile.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, AZKitMobile.INotificationsManager
    {
        //stores the returned device token so that it can be used for registering with the mobile service
        private static NSData _pushDeviceToken;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            //initialize azure mobile client platform to load all assemblies
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            LoadApplication(new App());

            RegisterForPushNotifications();
            

            return base.FinishedLaunching(app, options);
        }

        private void RegisterForPushNotifications()
        {
            //create the settings to allow push for alerts and badges
            //we don't currently use all these types, but they might get used
            var settings = UIUserNotificationSettings.GetSettingsForTypes(
                UIUserNotificationType.Alert
                | UIUserNotificationType.Badge
                | UIUserNotificationType.Sound,
                new NSSet());

            try
            {
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            catch (Exception ex)
            {
                UIAlertView warning = new UIAlertView("Registration error", "There was an error registering your device for remote push notifications. Check your project settings. \r\n" + ex.Message, null, "OK", null);
                warning.Show();
            }
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            //store the token so we have it later for callback
            _pushDeviceToken = deviceToken;
            //let the app know that device push registration succeeded
            Xamarin.Forms.MessagingCenter.Send<INotificationsManager>(this, Constants.KEY_MESSAGING_NOTIFICATIONS);
        }

        
        /// <summary>
        /// Handles alerts when the application is running.
        /// Othewise iOS handles them.
        /// </summary>
        /// <param name="application"></param>
        /// <param name="userInfo"></param>
        /// <param name="completionHandler"></param>
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo,  Action<UIBackgroundFetchResult> completionHandler)
        {
            NSDictionary aps = userInfo.ObjectForKey(new NSString("aps")) as NSDictionary;
            NSDictionary alertDetails = null;
            if(aps.ContainsKey(new NSString("alert")))
            {
                alertDetails = aps[new NSString("alert")] as NSDictionary;

            }
            if(alertDetails != null)
            {
                string title = (alertDetails[new NSString("title")] as NSString).ToString();
                string message = (alertDetails[new NSString("message")] as NSString).ToString();

                UIAlertView alert = new UIAlertView(title, message, null, "OK", null);
                alert.Show();
            }
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            UIAlertView alert = new UIAlertView("Registration Error", "Error registering for push notifications: " + error.Description, null, "OK", null);
            alert.Show();
        }

        public async Task RegisterForPushNotifications(MobileServiceClient client)
        {
            var template = CreateTemplatePayload();
            
            var pusher = client.GetPush();
            await pusher.RegisterAsync(_pushDeviceToken, template);
            
        }

        /// <summary>
        /// Creates the template configuration for the APN template.
        /// This dictates the format of our push message when it arrives.
        /// </summary>
        /// <returns></returns>
        private JObject CreateTemplatePayload()
        {
            JObject templateBody = new JObject();
            templateBody["body"] = Constants.TEMPLATE_IOS;

            JObject templates = new JObject();
            templates["updatesTemplate"] = templateBody;

            return templates;

        }
    }
}
