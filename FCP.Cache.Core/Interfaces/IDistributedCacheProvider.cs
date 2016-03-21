using System.Threading.Tasks;

namespace FCP.Cache
{
    /// <summary>
    /// base interface fro distributed cache provider
    /// </summary>
    public interface IDistributedCacheProvider : ICacheProvider<string>
    {
        #region async methods
        Task<TValue> GetAsync<TValue>(string key);

        /// <summary>
        /// Gets a value for the specified key and region
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="region">The cache region</param>
        /// <returns></returns>
        Task<TValue> GetAsync<TValue>(string key, string region);

        Task SetAsync<TValue>(string key, TValue value, CacheEntryOptions options);

        /// <summary>
        /// Sets a value for the specified key and region
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="region">The cache region</param>
        /// <returns></returns>
        Task SetAsync<TValue>(string key, TValue value, CacheEntryOptions options, string region);

        Task RemoveAsync(string key);

        /// <summary>
        /// Removes a value for the specified key and region
        /// </summary>
        /// <param name="key"></param>
        /// <param name="region">The cache region</param>
        /// <returns></returns>
        Task RemoveAsync(string key, string region);

        Task ClearAsync();

        /// <summary>
        /// Clears the cache region
        /// </summary>
        /// <param name="region">The cache region</param>
        Task ClearRegionAsync(string region);
        #endregion
    }
}
