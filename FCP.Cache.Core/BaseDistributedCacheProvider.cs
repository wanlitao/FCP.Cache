using System;
using System.Threading.Tasks;

namespace FCP.Cache
{
    public abstract class BaseDistributedCacheProvider : BaseCacheProvider<string>, IDistributedCacheProvider
    {
        #region Get
        public async Task<TValue> GetAsync<TValue>(string key)
        {
            return await GetAsync<TValue>(key, null).ConfigureAwait(false);
        }

        public virtual async Task<TValue> GetAsync<TValue>(string key, string region)
        {
            var cacheEntry = await GetCacheEntryAsync<TValue>(key, region).ConfigureAwait(false);

            if (cacheEntry != null && string.Compare(cacheEntry.Key, key, true) == 0)
            {
                return cacheEntry.Value;
            }
            return default(TValue);            
        }

        protected virtual async Task<CacheEntry<string, TValue>> GetCacheEntryAsync<TValue>(string key, string region)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            return await GetCacheEntryInternalAsync<TValue>(key, region).ConfigureAwait(false);
        }

        protected abstract Task<CacheEntry<string, TValue>> GetCacheEntryInternalAsync<TValue>(string key, string region);
        #endregion

        #region Set
        public async Task SetAsync<TValue>(string key, TValue value, CacheEntryOptions options)
        {
            await SetAsync<TValue>(key, value, options, null).ConfigureAwait(false);
        }

        public virtual async Task SetAsync<TValue>(string key, TValue value, CacheEntryOptions options, string region)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var cacheEntry = new CacheEntry<string, TValue>(key, region, value, options);

            await SetAsync(cacheEntry).ConfigureAwait(false);
        }

        protected virtual async Task SetAsync<TValue>(CacheEntry<string, TValue> entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            await SetInternalAsync(entry).ConfigureAwait(false);
        }

        protected abstract Task SetInternalAsync<TValue>(CacheEntry<string, TValue> entry);
        #endregion

        #region Remove
        public async Task RemoveAsync(string key)
        {
            await RemoveAsync(key, null).ConfigureAwait(false);
        }

        public virtual async Task RemoveAsync(string key, string region)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            await RemoveInternalAsync(key, region).ConfigureAwait(false);
        }

        protected abstract Task RemoveInternalAsync(string key, string region);
        #endregion

        #region Clear
        public abstract Task ClearAsync();

        public virtual async Task ClearRegionAsync(string region)
        {
            if (string.IsNullOrEmpty(region))
                throw new ArgumentNullException(nameof(region));

            await ClearRegionInternalAsync(region).ConfigureAwait(false);
        }

        protected abstract Task ClearRegionInternalAsync(string region);              
        #endregion
    }
}
