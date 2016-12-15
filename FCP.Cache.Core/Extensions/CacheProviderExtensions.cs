namespace FCP.Cache
{
    /// <summary>
    /// Cache Provider 扩展
    /// </summary>
    public static class CacheProviderExtensions
    {
        public static void Set<TKey, TValue>(this ICacheProvider<TKey> cacheProvider, TKey key, TValue value)
        {
            cacheProvider.Set<TValue>(key, value, new CacheEntryOptions());
        }

        public static void Set<TKey, TValue>(this ICacheProvider<TKey> cacheProvider, TKey key, TValue value, string region)
        {
            cacheProvider.Set<TValue>(key, value, new CacheEntryOptions(), region);
        }

        #region convert distributed cache
        public static IDistributedCacheProvider AsDistributedCache(this ICacheProvider<string> cacheProvider)
        {
            return new CacheProviderDistributedDecorator(cacheProvider);
        }
        #endregion
    }
}
