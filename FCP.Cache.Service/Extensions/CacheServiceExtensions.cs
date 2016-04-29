using System.Threading.Tasks;

namespace FCP.Cache.Service
{
    public static class CacheServiceExtensions
    {
        public static TValue GetOrAdd<TValue>(this ICacheService cacheService, string key, TValue value)
        {
            return cacheService.GetOrAdd<TValue>(key, value, new CacheEntryOptions());
        }

        public static TValue GetOrAdd<TValue>(this ICacheService cacheService, string key, TValue value, string region)
        {
            return cacheService.GetOrAdd<TValue>(key, value, new CacheEntryOptions(), region);
        }

        public static Task<TValue> GetOrAddAsync<TValue>(this ICacheService cacheService, string key, TValue value)
        {
            return cacheService.GetOrAddAsync<TValue>(key, value, new CacheEntryOptions());
        }

        public static Task<TValue> GetOrAddAsync<TValue>(this ICacheService cacheService, string key, TValue value, string region)
        {
            return cacheService.GetOrAddAsync<TValue>(key, value, new CacheEntryOptions(), region);
        }
    }
}
