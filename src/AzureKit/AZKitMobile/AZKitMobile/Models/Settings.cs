using Plugin.Settings.Abstractions;

namespace AZKitMobile.Models
{
    /// <summary>
    /// Helper class to store settings cross platform 
    /// using the CrossSettings plugin.
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get { return Plugin.Settings.CrossSettings.Current; }
        }
        public static string ServiceUrl
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(Constants.KEY_SETTING_URL, Constants.DEFAULT_URL_MOBILE_SERVICE);
            }
            set {
                AppSettings.AddOrUpdateValue<string>(Constants.KEY_SETTING_URL, value);
            }
        }

        public static bool GeneralNotifications
        {
            get { return AppSettings.GetValueOrDefault<bool>(Constants.KEY_SETTING_NOTIFICATION_GENERAL, false); }
            set { AppSettings.AddOrUpdateValue(Constants.KEY_SETTING_NOTIFICATION_GENERAL, value); }
        }

        public static bool SiteUpdateNotifications
        {
            get { return AppSettings.GetValueOrDefault<bool>(Constants.KEY_SETTING_NOTIFICATION_SITE, false); }
            set { AppSettings.AddOrUpdateValue(Constants.KEY_SETTING_NOTIFICATION_SITE, value); }
        }


        public static string GoogleCCMProjectIdentifer
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(Constants.KEY_SETTING_GCM_PROJECT_ID);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(Constants.KEY_SETTING_GCM_PROJECT_ID, value);
            }
        }
    }
}
