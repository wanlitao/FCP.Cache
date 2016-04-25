namespace FCP.Cache.Service
{
    public static class CacheServiceExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this ICacheService<TKey> cacheService, TKey key, TValue value)
        {
            return cacheService.GetOrAdd<TValue>(key, value, new CacheEntryOptions());
        }

        public static TValue GetOrAdd<TKey, TValue>(this ICacheService<TKey> cacheService, TKey key, TValue value, string region)
        {
            return cacheService.GetOrAdd<TValue>(key, value, new CacheEntryOptions(), region);
        }
    }
}
