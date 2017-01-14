using System.Threading.Tasks;

namespace AzureKit.Data
{
    public interface IUserProfileRepository
    {
        Task<UserNotificationChoices> GetUserNotificationChoices(string userEmail);
        Task SetEmailNotifications(string userEmail, bool isEnabled);
    }
}
