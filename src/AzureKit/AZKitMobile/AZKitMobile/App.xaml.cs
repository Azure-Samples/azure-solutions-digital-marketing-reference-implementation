using System;
using Xamarin.Forms;

//compile the xamarin forms xaml at build time to improve performance
[assembly: Xamarin.Forms.Xaml.XamlCompilation(Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]

namespace AZKitMobile
{
    /// <summary>
    /// The core application object used by Xamarin.Forms. 
    /// This application is always available in code and provides the root 
    /// point for accessing necessary objects.
    /// </summary>
    public partial class App : Application
    {
        private static azkitClient _client;


        static App()
        {
            _client = new azkitClient();
        }
        public App()
        {
            InitializeComponent();

            // The root page of the application
            //if configuration is needed for the service, show settings
            //otherwise go right to the main page.
            if (String.IsNullOrEmpty(Models.Settings.ServiceUrl) || Models.Settings.ServiceUrl == Constants.DEFAULT_URL_MOBILE_SERVICE)
            {
                MainPage = new NavigationPage(new Views.Settings());
            }
            else
            {
                MainPage = new NavigationPage(new Views.Main());
            }

            //register for messages indicating that push notifications can be configured
            Xamarin.Forms.MessagingCenter.Subscribe<INotificationsManager>(this, Constants.KEY_MESSAGING_NOTIFICATIONS, NotificationsReady);
        }

        /// <summary>
        /// Executes when a message is received indicating notifications
        /// have been setup on the client OS. Time to setup the same
        /// with the mobile apps backend.
        /// </summary>
        /// <param name="mgr">The INotificationsManager that raised the event</param>
        private async void NotificationsReady(INotificationsManager mgr)
        {
            try
            {
                //use the mobile client wrapper to register with the mobile app
                await _client.RegisterForNotificationsAsync();
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(() =>
                    ((NavigationPage)MainPage).CurrentPage.DisplayAlert("Push registration error", ex.Message, "OK"));
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        public azkitClient MobileClient
        {
            get { return _client; }
        }
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
