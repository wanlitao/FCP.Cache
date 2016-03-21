using System.Threading.Tasks;

namespace FCP.Cache
{
    /// <summary>
    /// 分布式 Cache Provider 扩展
    /// </summary>
    public static class DistributedCacheProviderExtensions
    {
        public static Task SetAsync<TValue>(this IDistributedCacheProvider cacheProvider, string key, TValue value)
        {
            return cacheProvider.SetAsync<TValue>(key, value, new CacheEntryOptions());
        }

        public static Task SetAsync<TValue>(this IDistributedCacheProvider cacheProvider, string key, TValue value, string region)
        {
            return cacheProvider.SetAsync<TValue>(key, value, new CacheEntryOptions(), region);
        }
    }
}
