namespace AZKitMobile.Models
{
    public class ViewModelBase
    {
        /// <summary>
        /// Pass through property that checks if the user has logged in.
        /// Used primarily to configure display and enable/disable commands.
        /// </summary>
        public bool IsLoggedIn
        {
            get { return ((AZKitMobile.App)App.Current).MobileClient.IsLoggedIn; }
        }

    }
}
