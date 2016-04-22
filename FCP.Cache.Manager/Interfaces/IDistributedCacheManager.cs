using System.Threading.Tasks;

namespace FCP.Cache.Manager
{
    public interface IDistributedCacheManager : IDistributedCacheProvider, ICacheManager<string>
    {
        Task<TValue> GetOrAddAsync<TValue>(string key, TValue value, CacheEntryOptions options);

        Task<TValue> GetOrAddAsync<TValue>(string key, TValue value, CacheEntryOptions options, string region);
    }
}
