namespace FCP.Cache.Manager
{
    public static class CacheManagerExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this ICacheManager<TKey> cacheManager, TKey key, TValue value)
        {
            return cacheManager.GetOrAdd<TValue>(key, value, new CacheEntryOptions());
        }

        public static TValue GetOrAdd<TKey, TValue>(this ICacheManager<TKey> cacheManager, TKey key, TValue value, string region)
        {
            return cacheManager.GetOrAdd<TValue>(key, value, new CacheEntryOptions(), region);
        }
    }
}
