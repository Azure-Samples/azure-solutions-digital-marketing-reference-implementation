namespace AZKitMobile.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new AZKitMobile.App());
            NotificationsManager mgr = new NotificationsManager();
            mgr.NotifyReadyForNotifications();
        }   
    }
}
