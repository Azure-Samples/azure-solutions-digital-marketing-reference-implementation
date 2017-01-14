using Microsoft.WindowsAzure.MobileServices;
using Nito.AsyncEx;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AZKitMobile.Models
{
    public class MainViewModel : ViewModelBase
    {
        private INavigation _nav;

        public MainViewModel(INavigation navigator)
        {
            _nav = navigator;

            MessagingCenter.Subscribe<IMobileServiceClient, string>(
                this, Constants.KEY_MESSAGING_PROFILE, (msc, s) => {
                    UserGreeting = s;
                    //remove the subscription
                    MessagingCenter.Unsubscribe<IMobileServiceClient, string>(this, Constants.KEY_MESSAGING_PROFILE);
            });
        }

        public void StartLoad()
        {
            if (Content?.IsCompleted ?? true)
            {
                // see https://msdn.microsoft.com/en-us/magazine/dn605875.aspx
                Content = NotifyTaskCompletion.Create(Load());
            }
        }

        private Task<List<Models.ContentModelBase>> Load()
        {
            return ((AZKitMobile.App)App.Current).MobileClient.GetContentAsync();
        }

        private INotifyTaskCompletion<List<ContentModelBase>> _content;
        public INotifyTaskCompletion<List<ContentModelBase>> Content {
            get
            {
                return _content;
            }
            private set
            {
                _content = value;
                RaisePropertyChanged();
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
                RaisePropertyChanged();
            }
        }

        private Command _loginCommand;
        public ICommand LoginCommand => _loginCommand ??
            (_loginCommand = new Command(async () => await Login(), CanLogin));

        private Command _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand ??
            (_refreshCommand = new Command((ignoredParam) => StartLoad(), (ignoredParam) => Content?.IsCompleted ?? true));

        private ContentModelBase _selectedItem;
        public ContentModelBase SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged();
                if (_selectedItem != null)
                {
                    _nav.PushAsync(new Views.ItemDialog(_selectedItem));
                }
            }
        }

        private async Task<MobileServiceUser> Login()
        {
            var result = await ((AZKitMobile.App)App.Current).MobileClient.LoginUserAsync();
            _loginCommand.ChangeCanExecute();
            return result;
        }

        private bool CanLogin()
        {
            return !IsLoggedIn;
        }

        private Command _settingsCommand;
        public ICommand SettingsCommand => _settingsCommand ??
            (_settingsCommand = new Command(async () => await GoToSettings()));

        public Task GoToSettings()
        {
            return _nav.PushAsync(new Views.Settings());
        }        
    }
}
