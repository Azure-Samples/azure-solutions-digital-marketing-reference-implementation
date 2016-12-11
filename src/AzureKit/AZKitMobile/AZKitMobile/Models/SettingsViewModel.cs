using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace AZKitMobile.Models
{
    /// <summary>
    /// View model for the settings page
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        private INavigation _nav;

        public SettingsViewModel(INavigation navigation)
        {
            _nav = navigation;
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
            set
            {
                Settings.ServiceUrl = value;

                RaisePropertyChanged();
                _saveCommand?.ChangeCanExecute();
            }
        }

        //Android only - display configured to show this only on android devices
        public string GCMProjectID
        {
            get { return Settings.GoogleCCMProjectIdentifer; }
            set { Settings.GoogleCCMProjectIdentifer = value; }
        }

        private Command _saveCommand;
        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new Command(SaveSettings, () => !string.IsNullOrEmpty(ServiceAddress)));

        private async void SaveSettings()
        {
            //notify the app to reload the mobile client with the new url
            ((AZKitMobile.App)App.Current).MobileClient.InitClient();

            //save the settings (beyond the storage handled in the property binding)
            //by calling to the back end to update the tags for push notifications
            if (IsLoggedIn)
            {
                try
                {
                    //update registration with the tags data
                    bool notificationsEnabled = await ((AZKitMobile.App)App.Current).MobileClient.RegisterForNotificationsAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    //this will display on the home screen indicating an error
                    MessagingCenter.Send<azkitClient, string>(((AZKitMobile.App)App.Current).MobileClient, Constants.KEY_MESSAGING_EXCEPTION, ex.Message);
                }
            }

            MessagingCenter.Send(this, Constants.KEY_MESSAGING_SETTINGS);

            //if we got here from the main screen, navigate back
            if (_nav.NavigationStack.Count > 1)
            {
                await _nav.PopAsync(true);
            }
            else
            {
                //otherwise, navigate to the main screen
                await _nav.PushAsync(new Views.Main());
            }
        }
    }
}
