using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

//register with the xamarin dependency service
[assembly: Xamarin.Forms.Dependency(typeof(AZKitMobile.iOS.LoginManager))]
namespace AZKitMobile.iOS
{
    /// <summary>
    /// Responsible for client specific login code
    /// Must run in the client context for UI integration to work correctly.
    /// </summary>
    public class LoginManager : AZKitMobile.ILoginManager
    {
        public async Task<MobileServiceUser> LoginAsync(IMobileServiceClient client)
        {
            var controller = UIKit.UIApplication.SharedApplication.KeyWindow.RootViewController;
            var user = await client.LoginAsync(controller, MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory);

            return user;
        }
    }
}
