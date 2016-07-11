namespace AzureKit.Caching
{
    public interface ICacheService
    {
        T GetItem<T>(string key) where T : class, new();

        void PutItem<T>(string key, T item);

        void PurgeItem(string v);
    }
}
