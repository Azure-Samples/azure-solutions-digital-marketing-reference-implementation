using Microsoft.WindowsAzure.MobileServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace AZKitMobile.Models
{
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private INavigation _nav;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel(INavigation navigator)
        {
            _nav = navigator;
            LoginCommand = new Command(Login, CanLogin);
            SettingsCommand = new Command(GoToSettings);

            MessagingCenter.Subscribe<IMobileServiceClient, string>(
                this, Constants.KEY_MESSAGING_PROFILE, (msc, s)=>{
                    UserGreeting = s;
                    //remove the subscription
                    MessagingCenter.Unsubscribe<IMobileServiceClient, string>(this, Constants.KEY_MESSAGING_PROFILE);
            });
        }

        public async void Load()
        {
            MessagingCenter.Subscribe<azkitClient, string>(this, Constants.KEY_MESSAGING_EXCEPTION, (msc, e) => {
                Device.BeginInvokeOnMainThread(() => {
                    App.Current.MainPage.DisplayAlert("Error loading data", e, "OK");
                });

            });
            try
            {
                Loading = true;
                var foundContent = await ((AZKitMobile.App)App.Current).MobileClient.GetContentAsync();
                Content = foundContent;  
                        
            }
            finally
            {
                Loading = false;
                MessagingCenter.Unsubscribe<azkitClient, string>(this, Constants.KEY_MESSAGING_EXCEPTION);
            }
        }

        private List<Models.ContentModelBase> _content;

        public List<Models.ContentModelBase> Content {
            get { return _content; }
            set {
                _content = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Content)));
            }
        }

        public string UserGreeting
        {
            get {
                var userName = ((AZKitMobile.App)App.Current).MobileClient.UserFirstName;
                if (!string.IsNullOrEmpty(userName))
                {
                    return string.Format("Welcome {0}", userName);
                }
                else
                {
                    return string.Empty;
                }
            }
            private set
            {
                //this setter is only used to notify the UI that the value
                //has changed. It should only be invoked from the message center
                //notifications
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UserGreeting)));
            }
        }

        private bool _isLoading;

        /// <summary>
        /// Indicates if data is currently loading
        /// Used to show activity indicator
        /// </summary>
        public bool Loading
        {
            get { return _isLoading; }
            set { _isLoading = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Loading)));
            }
        }

        public ICommand  LoginCommand { get; set; }

        private async void Login()
        {
           var user = await  ((AZKitMobile.App)App.Current).MobileClient.LoginUserAsync();
        }

        private bool CanLogin()
        {
            return !IsLoggedIn;
        }

        public ICommand SettingsCommand { get; set; }

        public void GoToSettings()
        {
            _nav.PushAsync(new Views.Settings());
        }

        
    }
}
