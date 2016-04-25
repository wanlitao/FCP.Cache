using System.Threading.Tasks;

namespace FCP.Cache.Service
{
    public static class DistributedCacheServiceExtensions
    {
        public static Task<TValue> GetOrAddAsync<TValue>(this IDistributedCacheService cacheService, string key, TValue value)
        {
            return cacheService.GetOrAddAsync<TValue>(key, value, new CacheEntryOptions());
        }

        public static Task<TValue> GetOrAddAsync<TValue>(this IDistributedCacheService cacheService, string key, TValue value, string region)
        {
            return cacheService.GetOrAddAsync<TValue>(key, value, new CacheEntryOptions(), region);
        }
    }
}
