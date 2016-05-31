using System;
using System.Threading.Tasks;

namespace FCP.Cache.Service
{
    public interface ICacheService : IDistributedCacheProvider
    {
        TValue GetOrAdd<TValue>(string key, Func<string, TValue> valueFactory, CacheEntryOptions options);

        TValue GetOrAdd<TValue>(string key, Func<string, TValue> valueFactory, CacheEntryOptions options, string region);

        Task<TValue> GetOrAddAsync<TValue>(string key, Func<string, Task<TValue>> valueAsyncFactory, CacheEntryOptions options);

        Task<TValue> GetOrAddAsync<TValue>(string key, Func<string, Task<TValue>> valueAsyncFactory, CacheEntryOptions options, string region);
    }
}
