using System;
using System.Threading.Tasks;

namespace FCP.Cache.Service
{
    public static class CacheServiceExtensions
    {
        public static TValue GetOrAdd<TValue>(this ICacheService cacheService, string key, Func<string, TValue> valueFactory)
        {
            return cacheService.GetOrAdd<TValue>(key, valueFactory, new CacheEntryOptions());
        }

        public static TValue GetOrAdd<TValue>(this ICacheService cacheService, string key, Func<string, TValue> valueFactory, string region)
        {
            return cacheService.GetOrAdd<TValue>(key, valueFactory, new CacheEntryOptions(), region);
        }

        public static Task<TValue> GetOrAddAsync<TValue>(this ICacheService cacheService, string key, Func<string, Task<TValue>> valueAsyncFactory)
        {
            return cacheService.GetOrAddAsync<TValue>(key, valueAsyncFactory, new CacheEntryOptions());
        }

        public static Task<TValue> GetOrAddAsync<TValue>(this ICacheService cacheService, string key, Func<string, Task<TValue>> valueAsyncFactory, string region)
        {
            return cacheService.GetOrAddAsync<TValue>(key, valueAsyncFactory, new CacheEntryOptions(), region);
        }
    }
}
