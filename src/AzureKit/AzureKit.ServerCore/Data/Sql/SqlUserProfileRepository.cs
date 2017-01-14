using AzureKit.Config;
using AzureKit.Data.Sql.Models;
using System.Data.Entity;
using System.Threading.Tasks;

namespace AzureKit.Data.Sql
{
    public class SqlUserProfileRepository : IUserProfileRepository
    {
        private readonly ISqlConfig _config;

        public SqlUserProfileRepository(ISqlConfig config)
        {
            _config = config;
        }

        public async Task<UserNotificationChoices> GetUserNotificationChoices(string userEmail)
        {
            if (_config.ConnectionName != null)
            {
                using (var ctx = new AzureKitDbContext(_config.ConnectionName))
                {
                    UserProfile profile = await ctx
                        .UserProfiles
                        .SingleOrDefaultAsync(u => u.ContactEmail == userEmail);

                    if (profile == null)
                    {
                        profile = new UserProfile
                        {
                            ContactEmail = userEmail
                        };
                        ctx.UserProfiles.Add(profile);
                        await ctx.SaveChangesAsync();
                    }

                    return new UserNotificationChoices
                    {
                        NotificationEmailsEnabled = profile.NotificationEmailsEnabled
                    };
                }
            }
            return null;
        }

        public async Task SetEmailNotifications(string userEmail, bool isEnabled)
        {
            if (_config.ConnectionName != null)
            {
                using (var ctx = new AzureKitDbContext(_config.ConnectionName))
                {
                    UserProfile profile = await ctx
                        .UserProfiles
                        .SingleOrDefaultAsync(u => u.ContactEmail == userEmail);

                    if (profile == null)
                    {
                        profile = new UserProfile
                        {
                            ContactEmail = userEmail
                        };
                        ctx.UserProfiles.Add(profile);
                    }

                    profile.NotificationEmailsEnabled = isEnabled;
                    await ctx.SaveChangesAsync();
                }
            }
        }
    }
}
