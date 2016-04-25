namespace FCP.Cache.Service
{
    public interface ICacheService<TKey> : ICacheProvider<TKey>
    {
        TValue GetOrAdd<TValue>(TKey key, TValue value, CacheEntryOptions options);

        TValue GetOrAdd<TValue>(TKey key, TValue value, CacheEntryOptions options, string region);
    }
}
