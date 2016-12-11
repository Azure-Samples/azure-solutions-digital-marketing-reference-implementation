
using Xamarin.Forms;

namespace AZKitMobile.Views
{
    public partial class Settings : ContentPage
    {
        public Settings()
        {
            InitializeComponent();
            this.BindingContext = new Models.SettingsViewModel(this.Navigation);
        }
    }
}
