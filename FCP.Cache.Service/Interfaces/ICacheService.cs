using System.Threading.Tasks;

namespace FCP.Cache.Service
{
    public interface ICacheService : IDistributedCacheProvider
    {
        TValue GetOrAdd<TValue>(string key, TValue value, CacheEntryOptions options);

        TValue GetOrAdd<TValue>(string key, TValue value, CacheEntryOptions options, string region);

        Task<TValue> GetOrAddAsync<TValue>(string key, TValue value, CacheEntryOptions options);

        Task<TValue> GetOrAddAsync<TValue>(string key, TValue value, CacheEntryOptions options, string region);
    }
}
