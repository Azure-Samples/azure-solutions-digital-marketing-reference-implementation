using Android.App;
using Android.Content.PM;
using Android.OS;
using Gcm.Client;


namespace AZKitMobile.Droid
{
    [Activity(Label = "AZKitMobile", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //intialize the azure mobile platform
            //reguired on this platform to load all assemblies
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());

           //only register for notifications if we have a gcm project identifier in the settings
            if (!string.IsNullOrEmpty(AZKitMobile.Models.Settings.GoogleCCMProjectIdentifer))
            {
                GcmClient.CheckDevice(this.ApplicationContext);
                GcmClient.CheckManifest(this.ApplicationContext);

                GcmClient.Register(this.ApplicationContext, AZKitMobile.Models.Settings.GoogleCCMProjectIdentifer);
            }
        }
    }
}

