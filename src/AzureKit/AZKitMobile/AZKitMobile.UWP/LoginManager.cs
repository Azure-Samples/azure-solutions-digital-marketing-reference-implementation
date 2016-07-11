using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

//register with the Xamarin dependency service
[assembly:Xamarin.Forms.Dependency(typeof(AZKitMobile.UWP.LoginManager))]
namespace AZKitMobile.UWP
{
    /// <summary>
    /// Responsible for the client specific login calls.
    /// Must run in this context to use the platform specific extensions.
    /// </summary>
    public class LoginManager : AZKitMobile.ILoginManager
    {
        public async Task<MobileServiceUser> LoginAsync(IMobileServiceClient client)
        {
            var user = await client.LoginAsync(MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory,false);

            return user;
        }
    }
}
