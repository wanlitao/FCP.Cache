using System.Threading.Tasks;

namespace FCP.Cache.Service
{
    public interface IDistributedCacheService : IDistributedCacheProvider, ICacheService<string>
    {
        Task<TValue> GetOrAddAsync<TValue>(string key, TValue value, CacheEntryOptions options);

        Task<TValue> GetOrAddAsync<TValue>(string key, TValue value, CacheEntryOptions options, string region);
    }
}
