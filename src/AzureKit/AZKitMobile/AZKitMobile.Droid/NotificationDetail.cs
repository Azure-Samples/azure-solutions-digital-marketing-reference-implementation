using Android.App;
using Android.OS;
using Android.Widget;

namespace AZKitMobile.Droid
{
    /// <summary>
    /// Activity to display details of the notification / alert
    /// </summary>
    [Activity(Label = "NotificationDetail")]
    public class NotificationDetail : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.NotificationDetail);
            // Create your application here
            
            var message = this.Intent.GetStringExtra("message"); 
            var title = this.Intent.GetStringExtra("title");

            var titleView = FindViewById<TextView>(Resource.Id.titleView);
            titleView.Text = title;

            var messageView = FindViewById<TextView>(Resource.Id.messageView);
            messageView.Text = message;
           
        }

    }
}