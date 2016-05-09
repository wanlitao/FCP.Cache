using System;

namespace FCP.Cache
{
    /// <summary>
    /// base interface for cache provider
    /// </summary>
    /// <typeparam name="TKey"></typeparam>    
    public interface ICacheProvider<TKey> : IDisposable
    {
        TValue Get<TValue>(TKey key);

        /// <summary>
        /// Gets a value for the specified key and region
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="region">The cache region</param>
        /// <returns></returns>
        TValue Get<TValue>(TKey key, string region);

        CacheEntry<TKey, TValue> GetCacheEntry<TValue>(TKey key);

        CacheEntry<TKey, TValue> GetCacheEntry<TValue>(TKey key, string region);

        void Set<TValue>(TKey key, TValue value, CacheEntryOptions options);

        /// <summary>
        /// Sets a value for the specified key and region
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="region">The cache region</param>
        /// <returns></returns>
        void Set<TValue>(TKey key, TValue value, CacheEntryOptions options, string region);

        void Set<TValue>(CacheEntry<TKey, TValue> entry);

        void Remove(TKey key);

        /// <summary>
        /// Removes a value for the specified key and region
        /// </summary>
        /// <param name="key"></param>
        /// <param name="region">The cache region</param>
        /// <returns></returns>
        void Remove(TKey key, string region);

        void Clear();

        /// <summary>
        /// Clears the cache region
        /// </summary>
        /// <param name="region">The cache region</param>
        void ClearRegion(string region);
    }
}
