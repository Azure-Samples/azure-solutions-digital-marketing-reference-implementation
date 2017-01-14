using AzureKit.Config;
using System.Configuration;

namespace AzureKit.Caching
{
    /// <summary>
    /// Supplies the Redis Cache output caching provider with a connection string from
    /// the application settings.
    /// </summary>
    /// <remarks>
    /// We need the RedisOutputCacheProvider class to get its connection string from
    /// application settings, so that Azure Web Apps can get the right Redis cache
    /// for their environment. Most RedisOutputCacheProvider examples bake the
    /// connection string directly into the Web.config in the caching/outputCache/providers
    /// section, which leaves Web.config transforms as the only way to change it.
    /// A better approach is to use the RedisOutputCacheProvider's ability to read
    /// the setting from a named method and class at runtime, which lets us retrieve
    /// the value from wherever we want.
    /// </remarks>
    public static class RedisCacheOutputProviderSettingsSource
    {
        public static string GetConnectionString() =>
            ConfigurationManager.AppSettings[Constants.KEY_AZURE_REDIS_CONNECTION];
    }
}