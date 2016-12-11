using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AZKitMobile.Models
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Pass through property that checks if the user has logged in.
        /// Used primarily to configure display and enable/disable commands.
        /// </summary>
        public bool IsLoggedIn
        {
            get { return ((AZKitMobile.App)App.Current).MobileClient.IsLoggedIn; }
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
