using System;
using System.Threading.Tasks;

namespace FCP.Cache.Service
{
    public abstract class BaseDistributedCacheService : BaseCacheService<string>, IDistributedCacheService
    {
        #region Get
        public Task<TValue> GetAsync<TValue>(string key)
        {
            return GetAsync<TValue>(key, null);
        }

        public virtual Task<TValue> GetAsync<TValue>(string key, string region)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetOrAdd
        public Task<TValue> GetOrAddAsync<TValue>(string key, TValue value, CacheEntryOptions options)
        {
            return GetOrAddAsync(key, value, options, null);
        }

        public virtual Task<TValue> GetOrAddAsync<TValue>(string key, TValue value, CacheEntryOptions options, string region)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Set
        public Task SetAsync<TValue>(string key, TValue value, CacheEntryOptions options)
        {
            return SetAsync(key, value, options, null);
        }

        public virtual Task SetAsync<TValue>(string key, TValue value, CacheEntryOptions options, string region)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Remove
        public Task RemoveAsync(string key)
        {
            return RemoveAsync(key, null);
        }

        public virtual Task RemoveAsync(string key, string region)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Clear
        public virtual Task ClearAsync()
        {
            throw new NotImplementedException();
        }

        public virtual Task ClearRegionAsync(string region)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
