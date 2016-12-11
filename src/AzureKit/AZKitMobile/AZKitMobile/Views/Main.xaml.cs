
using AZKitMobile.Models;
using Xamarin.Forms;

namespace AZKitMobile.Views
{
    public partial class Main : ContentPage
    {
        private MainViewModel ViewModel
        {
            get { return BindingContext as MainViewModel; }
            set { BindingContext = value; }
        }

        public Main()
        {
            InitializeComponent();

            // Unfortunately it's not possible to add toolbar items conditionally in XAML
            if (Device.OS == TargetPlatform.WinPhone || Device.OS == TargetPlatform.Windows)
            {
                var refreshButton = new ToolbarItem
                {
                    Order = ToolbarItemOrder.Primary,
                    Text = "Refresh",
                    Icon = "toolbar_refresh.png"                    
                };

                var commandBinding = new Binding("RefreshCommand");
                refreshButton.SetBinding(ToolbarItem.CommandProperty, commandBinding);

                ToolbarItems.Add(refreshButton);
            }

            MessagingCenter.Subscribe<azkitClient, string>(this, Constants.KEY_MESSAGING_EXCEPTION, async (msc, e) =>
            {
                await DisplayAlert("An error occurred", e, "OK");
            });

            ViewModel = new MainViewModel(this.Navigation);
            ViewModel.StartLoad();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.SelectedItem = null;
        }
    }
}
