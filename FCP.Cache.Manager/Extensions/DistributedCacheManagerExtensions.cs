using System.Threading.Tasks;

namespace FCP.Cache.Manager
{
    public static class DistributedCacheManagerExtensions
    {
        public static Task<TValue> GetOrAddAsync<TValue>(this IDistributedCacheManager cacheManager, string key, TValue value)
        {
            return cacheManager.GetOrAddAsync<TValue>(key, value, new CacheEntryOptions());
        }

        public static Task<TValue> GetOrAddAsync<TValue>(this IDistributedCacheManager cacheManager, string key, TValue value, string region)
        {
            return cacheManager.GetOrAddAsync<TValue>(key, value, new CacheEntryOptions(), region);
        }
    }
}
