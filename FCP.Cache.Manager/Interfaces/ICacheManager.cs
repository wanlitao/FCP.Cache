namespace FCP.Cache.Manager
{
    public interface ICacheManager<TKey> : ICacheProvider<TKey>
    {
        TValue GetOrAdd<TValue>(TKey key, TValue value, CacheEntryOptions options);

        TValue GetOrAdd<TValue>(TKey key, TValue value, CacheEntryOptions options, string region);
    }
}
