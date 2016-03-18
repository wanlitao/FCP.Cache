using System.Threading.Tasks;

namespace FCP.Cache
{
    /// <summary>
    /// base interface fro distributed cache provider
    /// </summary>
    public interface IDistributedCacheProvider<TKey> : ICacheProvider<TKey>
    {
        #region async methods
        Task<TValue> GetAsync<TValue>(TKey key);

        /// <summary>
        /// Gets a value for the specified key and region
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="region">The cache region</param>
        /// <returns></returns>
        Task<TValue> GetAsync<TValue>(TKey key, string region);

        Task<bool> SetAsync<TValue>(TKey key, TValue value);

        /// <summary>
        /// Sets a value for the specified key and region
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="region">The cache region</param>
        /// <returns></returns>
        Task<bool> SetAsync<TValue>(TKey key, TValue value, string region);

        Task<bool> RemoveAsync(TKey key);

        /// <summary>
        /// Removes a value for the specified key and region
        /// </summary>
        /// <param name="key"></param>
        /// <param name="region">The cache region</param>
        /// <returns></returns>
        Task<bool> RemoveAsync(TKey key, string region);

        Task ClearAsync();

        /// <summary>
        /// Clears the cache region
        /// </summary>
        /// <param name="region">The cache region</param>
        Task ClearRegionAsync(string region);
        #endregion
    }
}
