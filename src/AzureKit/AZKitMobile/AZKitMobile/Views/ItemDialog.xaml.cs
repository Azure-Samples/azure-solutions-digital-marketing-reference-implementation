using System;

using Xamarin.Forms;

namespace AZKitMobile.Views
{
    public partial class ItemDialog : ContentPage
    {
        public ItemDialog(string htmlContent)
        {
            InitializeComponent();
            HtmlWebViewSource source =
                new HtmlWebViewSource();
            source.Html = "<html><body>" + htmlContent + "</body></html>";

            contentViewer.Source = source;
        }

        public async void CloseDialog(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
