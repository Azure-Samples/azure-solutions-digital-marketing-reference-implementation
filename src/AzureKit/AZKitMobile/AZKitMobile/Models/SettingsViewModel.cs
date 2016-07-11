using System.Windows.Input;
using Xamarin.Forms;

namespace AZKitMobile.Models
{
    /// <summary>
    /// View model for the settings page
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        private INavigation nav;

        public SettingsViewModel(INavigation navigation)
        {
            nav = navigation;
            SaveCommand = new Xamarin.Forms.Command(SaveSettings);
        }
        public bool NotificationGeneral
        {
            get { return Settings.GeneralNotifications; }
            set { Settings.GeneralNotifications = value; }
        }
        public bool NotificationSite {
            get { return Settings.SiteUpdateNotifications; }
            set { Settings.SiteUpdateNotifications = value; }
        }

        public string ServiceAddress {
            get { return Settings.ServiceUrl; }
            set { Settings.ServiceUrl = value; }
        }

        //Android only - display configured to show this only on android devices
        public string GCMProjectID
        {
            get { return Settings.GoogleCCMProjectIdentifer; }
            set { Settings.GoogleCCMProjectIdentifer = value; }
        }
        public ICommand SaveCommand { get; set; }

        private async void SaveSettings()
        {
            //save the settings (beyond the storage handled in the property binding)
            //by calling to the back end to update the tags for push notifications
            if(IsLoggedIn)
            {              
               //update registration with the tags data
                bool notificationsEnabled = await ((AZKitMobile.App)App.Current).MobileClient.RegisterForNotificationsAsync();   
            }
            //if we got here from the main screen, navigate back
            if (nav.NavigationStack.Count > 1)
            {
                await nav.PopAsync(true);
            }
            else
            {
                //otherwise, navigate to the main screen
                await nav.PushAsync(new Views.Main());
            }
        }
    }
}
