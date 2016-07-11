using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace AZKitMobile
{
    /// <summary>
    /// Defines the interface clients must implement and register with the
    /// dependency service so client specific login calls can happen.
    /// </summary>
    public interface ILoginManager
    {
        /// <summary>
        /// Invoked by the shared code to run login in the client application code
        /// since the signatures and implementations differ with client
        /// extensions.
        /// </summary>
        /// <param name="client">The mobile service client to use to login.</param>
        /// <returns></returns>
        Task<MobileServiceUser> LoginAsync(IMobileServiceClient client);
    }
}
