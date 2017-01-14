using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace AZKitMobile
{
    /// <summary>
    /// The interface clients must implement in order to do client 
    /// registration for push notifications with the mobile backend.
    /// </summary>
    public interface INotificationsManager
    {
        /// <summary>
        /// Invoked by the shared code to get client specific push code to 
        /// execute in the client app. 
        /// </summary>
        /// <param name="client">The client to use for registering. 
        /// Note this is not using the IMobileServiceClient interface because that interface does not have the necessary push methods defined/exposed.</param>
        /// <returns>True if the client registration succeeds so that tags can then be updated in shared code.</returns>
        Task RegisterForPushNotifications(MobileServiceClient client);
    }
}
