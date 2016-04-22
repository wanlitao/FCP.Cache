using System;
using System.Threading.Tasks;

namespace FCP.Cache
{
    public abstract class BaseDistributedCacheProvider : BaseCacheProvider<string>, IDistributedCacheProvider
    {
        #region Get
        public Task<TValue> GetAsync<TValue>(string key)
        {
            return GetAsync<TValue>(key, null);
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

        protected virtual Task<CacheEntry<string, TValue>> GetCacheEntryAsync<TValue>(string key, string region)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            CheckDisposed();

            return GetCacheEntryInternalAsync<TValue>(key, region);
        }

        protected abstract Task<CacheEntry<string, TValue>> GetCacheEntryInternalAsync<TValue>(string key, string region);
        #endregion

        #region Set
        public Task SetAsync<TValue>(string key, TValue value, CacheEntryOptions options)
        {
            return SetAsync<TValue>(key, value, options, null);
        }

        public virtual Task SetAsync<TValue>(string key, TValue value, CacheEntryOptions options, string region)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var cacheEntry = new CacheEntry<string, TValue>(key, region, value, options);

            return SetAsync(cacheEntry);
        }

        protected virtual Task SetAsync<TValue>(CacheEntry<string, TValue> entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            CheckDisposed();

            return SetInternalAsync(entry);
        }

        protected abstract Task SetInternalAsync<TValue>(CacheEntry<string, TValue> entry);
        #endregion

        #region Remove
        public Task RemoveAsync(string key)
        {
            return RemoveAsync(key, null);
        }

        public virtual Task RemoveAsync(string key, string region)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            CheckDisposed();

            return RemoveInternalAsync(key, region);
        }

        protected abstract Task RemoveInternalAsync(string key, string region);
        #endregion

        #region Clear
        public virtual Task ClearAsync()
        {
            CheckDisposed();

            return ClearInternalAsync();
        }

        protected abstract Task ClearInternalAsync();


        public virtual Task ClearRegionAsync(string region)
        {
            if (string.IsNullOrEmpty(region))
                throw new ArgumentNullException(nameof(region));

            CheckDisposed();

            return ClearRegionInternalAsync(region);
        }

        protected abstract Task ClearRegionInternalAsync(string region);              
        #endregion
    }
}
