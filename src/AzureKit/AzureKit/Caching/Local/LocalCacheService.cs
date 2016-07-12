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
        private static ConcurrentDictionary<string, object> _cacheData;

        static LocalCacheService()
        {
            _cacheData = new ConcurrentDictionary<string, object>();
        }

        public T GetItem<T>(string key) where T : class,new()
        {
            object foundItem = null;
            if (_cacheData.TryGetValue(key, out foundItem))
            {
                return (T)foundItem;
            }
            else
                return null;
        }

        public void PurgeItem(string key)
        {
           object removedItem;
           _cacheData.TryRemove(key, out removedItem);     
        }

        public void PutItem<T>(string key, T item)
        {
            _cacheData.AddOrUpdate(key, item, (k, v) => v);
        }   
    }
}