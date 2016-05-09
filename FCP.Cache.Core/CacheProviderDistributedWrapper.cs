using System;
using System.Threading.Tasks;

namespace FCP.Cache
{
    /// <summary>
    /// Distributed Wrapper for Cache Provider
    /// </summary>
    internal class CacheProviderDistributedWrapper : IDistributedCacheProvider
    {
        internal static readonly Task CompletedTask = Task.FromResult<object>(null);

        private ICacheProvider<string> _localCacheProvider;

        internal CacheProviderDistributedWrapper(ICacheProvider<string> cacheProvider)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));

            _localCacheProvider = cacheProvider;
        }        

        #region Get
        public TValue Get<TValue>(string key)
        {
            return _localCacheProvider.Get<TValue>(key);
        }

        public TValue Get<TValue>(string key, string region)
        {
            return _localCacheProvider.Get<TValue>(key, region);
        }

        public CacheEntry<string, TValue> GetCacheEntry<TValue>(string key)
        {
            return _localCacheProvider.GetCacheEntry<TValue>(key);
        }

        public CacheEntry<string, TValue> GetCacheEntry<TValue>(string key, string region)
        {
            return _localCacheProvider.GetCacheEntry<TValue>(key, region);
        }

        public Task<TValue> GetAsync<TValue>(string key)
        {
            return Task.FromResult(Get<TValue>(key));
        }

        public Task<TValue> GetAsync<TValue>(string key, string region)
        {
            return Task.FromResult(Get<TValue>(key, region));
        }

        public Task<CacheEntry<string, TValue>> GetCacheEntryAsync<TValue>(string key)
        {
            return Task.FromResult(GetCacheEntry<TValue>(key));
        }

        public Task<CacheEntry<string, TValue>> GetCacheEntryAsync<TValue>(string key, string region)
        {
            return Task.FromResult(GetCacheEntry<TValue>(key, region));
        }
        #endregion

        #region Set
        public void Set<TValue>(string key, TValue value, CacheEntryOptions options)
        {
            _localCacheProvider.Set<TValue>(key, value, options);
        }

        public void Set<TValue>(string key, TValue value, CacheEntryOptions options, string region)
        {
            _localCacheProvider.Set<TValue>(key, value, options, region);
        }

        public void Set<TValue>(CacheEntry<string, TValue> entry)
        {
            _localCacheProvider.Set<TValue>(entry);
        }

        public Task SetAsync<TValue>(string key, TValue value, CacheEntryOptions options)
        {
            Set<TValue>(key, value, options);

            return CompletedTask;
        }

        public Task SetAsync<TValue>(string key, TValue value, CacheEntryOptions options, string region)
        {
            Set<TValue>(key, value, options, region);

            return CompletedTask;
        }

        public Task SetAsync<TValue>(CacheEntry<string, TValue> entry)
        {
            Set<TValue>(entry);

            return CompletedTask;
        }
        #endregion

        #region Remove
        public void Remove(string key)
        {
            _localCacheProvider.Remove(key);
        }

        public void Remove(string key, string region)
        {
            _localCacheProvider.Remove(key, region);
        }

        public Task RemoveAsync(string key)
        {
            Remove(key);

            return CompletedTask;
        }

        public Task RemoveAsync(string key, string region)
        {
            Remove(key, region);

            return CompletedTask;
        }
        #endregion

        #region Clear
        public void Clear()
        {
            _localCacheProvider.Clear();
        }

        public Task ClearAsync()
        {
            Clear();

            return CompletedTask;
        }

        public void ClearRegion(string region)
        {
            _localCacheProvider.ClearRegion(region);
        }

        public Task ClearRegionAsync(string region)
        {
            ClearRegion(region);

            return CompletedTask;
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _localCacheProvider.Dispose();
                }
                disposedValue = true;
            }
        }
        
        ~CacheProviderDistributedWrapper() {         
            Dispose(false);
        }
        
        public void Dispose()
        {            
            Dispose(true);            
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
