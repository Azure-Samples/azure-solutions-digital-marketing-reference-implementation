using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(AZKitMobile.Droid.LoginManager))]
namespace AZKitMobile.Droid
{
    public class LoginManager : AZKitMobile.ILoginManager
    {
        public async Task<MobileServiceUser> LoginAsync(IMobileServiceClient client)
        {
            try
            {
                var user = await client.LoginAsync(
                    Xamarin.Forms.Forms.Context,
                    MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory);

                return user;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.Message);
                return null;
            }
        }
    }
}