using AZKitMobile.Models;
using System;

using Xamarin.Forms;

namespace AZKitMobile.Views
{
    public partial class ItemDialog : ContentPage
    {
        public ItemDialog(ContentModelBase item)
        {
            InitializeComponent();

            if (Device.OS == TargetPlatform.WinPhone || Device.OS == TargetPlatform.Windows)
            {
                var closeButton = new ToolbarItem
                {
                    Text = "Close",
                    Icon = "toolbar_close.png"                
                };

                closeButton.Clicked += CloseDialog;

                ToolbarItems.Add(closeButton);
            }

            Title = item.Title;
            var source = new HtmlWebViewSource
            {
                // TODO: you could add some inline CSS here to make the 
                // content look a little bit better, or put an HTML
                // template with inline CSS in an embedded resource
                Html = "<html><head><style>body { margin: 10px; }</style></head><body>" + item.Content + "</body></html>"
            };

            contentViewer.Source = source;
        }

        public async void CloseDialog(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
//            await Navigation.PopModalAsync();
        }
    }
}
