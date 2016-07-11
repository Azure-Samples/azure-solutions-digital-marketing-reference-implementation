
using System;
using Xamarin.Forms;

namespace AZKitMobile.Views
{
    public partial class Main : ContentPage
    {
        public Main()
        {
            InitializeComponent();
            var model = new Models.MainViewModel(this.Navigation);

            this.BindingContext = model;

            try
            {
                model.Load();
            }
            catch(Exception ex)
            {
                this.DisplayAlert("Error loading content", ex.Message, "OK");
            }
           
        }

        async void OnSelection(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                Models.ContentModelBase model = (Models.ContentModelBase) e.SelectedItem;
                await Navigation.PushModalAsync(
                    new Views.ItemDialog(model.Content));
            }
           
        }
    
    }
}
