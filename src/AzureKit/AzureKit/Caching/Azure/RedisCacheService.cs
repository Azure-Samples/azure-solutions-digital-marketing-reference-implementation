using AzureKit.Config;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Configuration;

namespace AzureKit.Caching.Azure
{
    /// <summary>
    /// Cache implemenation that uses redis to store content and site map
    /// </summary>
    public class RedisCacheService : ICacheService
    {
        // Redis Connection string info
        private static Lazy<ConnectionMultiplexer> s_lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheConnection = ConfigurationManager.AppSettings[Constants.KEY_AZURE_REDIS_CONNECTION].ToString();
            return ConnectionMultiplexer.Connect(cacheConnection);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return s_lazyConnection.Value;
            }
        }

        private IDatabase Cache { get { return Connection.GetDatabase(); } }

        public RedisCacheService()
        { }
       
        public T GetItem<T>(string key) where T : class, new()
        {
            var db = Cache;
            if (db != null && db.KeyExists(key))
            {
                var cacheItem = db.StringGet(key);
                //use type information so we can serialize/deserialize correct types
                JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
                var item = JsonConvert.DeserializeObject<T>(cacheItem, settings);
                return item;
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Removes an item from the cache
        /// </summary>
        /// <param name="key">The key used to identify the item to be removed.</param>
        public void PurgeItem(string key)
        {
            var db = Cache;
            if(db!= null)
            {
                db.KeyDelete(key);
            }
        }

        /// <summary>
        /// Add or update an item in the cache
        /// </summary>
        /// <typeparam name="T">The type of the item being saved</typeparam>
        /// <param name="key">The key used to identify the item in the cache</param>
        /// <param name="item">The item to store</param>
        public void PutItem<T>(string key, T item)
        {
            var db = Cache;
            if (db != null)
            {
                JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
                db.StringSet(key, JsonConvert.SerializeObject(item, settings));
            }
        }
    }
}