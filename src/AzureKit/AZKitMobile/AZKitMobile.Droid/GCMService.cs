using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Gcm.Client;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]

//GET_ACCOUNTS is only needed for android versions 4.0.3 and below
//[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]
[assembly: UsesPermission(Name = "android.permission.RECEIVE_BOOT_COMPLETED")]

//register the notifications manager dependency with the xamarin.forms dependency service
[assembly: Xamarin.Forms.Dependency(typeof(AZKitMobile.Droid.GCMService))]
namespace AZKitMobile.Droid
{
    [BroadcastReceiver(Permission = Gcm.Client.Constants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new[] { Android.Content.Intent.ActionBootCompleted })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Gcm.Client.Constants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "@PACKAGE_NAME@" })]
    public class GCMBroadcastReceiver : GcmBroadcastReceiverBase<GCMService>
    { }

    /// <summary>
    /// This is the service that handles push registration and messages.
    /// </summary>
    [Service]
    public class GCMService : GcmServiceBase, AZKitMobile.INotificationsManager
    {
        private static string _deviceIdentifier;

		public GCMService () :base(AZKitMobile.Models.Settings.GoogleCCMProjectIdentifer)
		{}

        /// <summary>
        /// Called from shared code to register with the mobile app push service.
        /// </summary>
        /// <param name="client">The client to use to register for push notifications.</param>
        /// <returns>true if registration succeeds.</returns>
        public async Task RegisterForPushNotifications(MobileServiceClient client)
        {
            var template = CreateTemplatePayload();
            await client.GetPush().RegisterAsync(_deviceIdentifier, template);
        }

        /// <summary>
        /// Creates the gcm specific template for push notifications.
        /// This is the format in which the message will now arrive.
        /// </summary>
        /// <returns></returns>
        private JObject CreateTemplatePayload()
        {
            JObject templateBody = new JObject();
            templateBody["body"] = Constants.TEMPLATE_DROID;

            JObject templates = new JObject();
            templates["updatesTemplate"] = templateBody;

            return templates;

        }

        /// <summary>
        /// invoked when a push message arrives.
        /// </summary>
        /// <param name="context">An app/activity context to work with</param>
        /// <param name="intent">The intent with data in it</param>
        protected override void OnMessage(Context context, Intent intent)
        {
            //extract the data from the intent
			string title = intent.GetStringExtra ("title");
			string message = intent.GetStringExtra ("message");
            
            //build up the notification details
			NotificationCompat.Builder messageBuilder = 
				new NotificationCompat.Builder (context)
					.SetContentTitle (title)
					.SetContentText (message)
					.SetPriority (0)
					.SetSmallIcon (Resource.Drawable.icon)
                    .SetAutoCancel(true);
			NotificationManager notificationMgr = 
				(NotificationManager)context.GetSystemService (
					Context.NotificationService);

            //Create activity intent to show details when the alert is tapped
            Intent display = new Intent(context, Java.Lang.Class.FromType(typeof(NotificationDetail)));          
            display.PutExtra("title", title);
            display.PutExtra("message", message);
                        

            PendingIntent pDisplay = PendingIntent.GetActivity(context, 0, display, PendingIntentFlags.UpdateCurrent);

            messageBuilder.SetContentIntent(pDisplay);

            //add to the alert center
			notificationMgr.Notify (0, messageBuilder.Build ());
        }

        /// <summary>
        /// Invoked when the device has registered with GCM
        /// </summary>
        /// <param name="context">The context to work with</param>
        /// <param name="registrationId">The unique reigstration id with GCM</param>
        protected override void OnRegistered(Context context, string registrationId)
        {
            //save the identifier so we can use it on callback
            _deviceIdentifier = registrationId;
            // notify the app that it is okay to register for notifications
            Xamarin.Forms.MessagingCenter.Send<INotificationsManager>(this, Constants.KEY_MESSAGING_NOTIFICATIONS);        
        }

		protected override void OnError (Context context, string errorId)
		{
			Console.Write (errorId);
		}
		protected override void OnUnRegistered (Context context, string registrationId)
		{

		}
    }
}