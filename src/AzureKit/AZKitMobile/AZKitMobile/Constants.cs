namespace AZKitMobile
{
    public static class Constants
    {
        //TODO: Set to your azure mobile app URL (your web app address) 
        //this makes it easier to get started in the apps instead of typing it into the settings page
        // https://{yourwebapp}.azurewebsites.net/
        public static readonly string DEFAULT_URL_MOBILE_SERVICE = "";

        public static readonly string KEY_SETTING_URL = "key_baseUrl";
        public static readonly string KEY_SETTING_NOTIFICATION_GENERAL = "key_generalNotifications";
        public static readonly string KEY_SETTING_NOTIFICATION_SITE = "key_siteUpdateNotifications";
        public static readonly string KEY_SETTING_GCM_PROJECT_ID = "key_gcmProjectId";

        public static readonly string TAG_GENERAL = "General";
        public static readonly string TAG_SITE = "SiteUpdates";


        public static readonly string TEMPLATE_IOS = "{\"aps\": {\"alert\":\"{$(title) + \': \' + $(message)}\"}}";
        public static readonly string TEMPLATE_DROID = "{\"data\":{\"title\":\"$(title)\", \"message\":\"$(message)\"}}";
        public static readonly string TEMPLATE_UWP = @"<toast><visual><binding template=""ToastText02""><text id=""1"">"
                    + @"$(title)</text><text id=""2"">"
                    + @"$(message)</text></binding></visual></toast>";

        public static readonly string KEY_MESSAGING_NOTIFICATIONS = "NotificationsReady";
        public static readonly string KEY_MESSAGING_PROFILE = "ProfileLoaded";
        public static readonly string KEY_MESSAGING_EXCEPTION = "LoadException";
        public static readonly string KEY_MESSAGING_SETTINGS = "SettingsSaved";


    }
}
