using System.Collections.Concurrent;

namespace AzureKit.Caching.Local
{
    /// <summary>
    /// In Memory cache service useful for development or single server
    /// implementations. Configured in the DependencyResolution to return 
    /// a singleton instance.
    /// </summary>
    public class LocalCacheService : ICacheService
    {
        private static ConcurrentDictionary<string, object> cacheData;

        static LocalCacheService()
        {
            cacheData = new ConcurrentDictionary<string, object>();
        }

        public T GetItem<T>(string key) where T : class,new()
        {
            if (cacheData.ContainsKey(key))
            {
                return (T)cacheData[key];
            }
            else
                return null;
        }

        public void PurgeItem(string key)
        {
            if(cacheData.ContainsKey(key))
            {
                object removedItem;
                cacheData.TryRemove(key, out removedItem);
            }
        }

        public void PutItem<T>(string key, T item)
        {
            if (cacheData.ContainsKey(key))
                cacheData[key] = item;
            else
                cacheData.TryAdd(key, item);
        }

        
    }
}