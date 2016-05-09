using System;
using System.Threading.Tasks;

namespace FCP.Cache.Service
{
    public abstract class BaseCacheService : BaseDistributedCacheProvider, ICacheService
    {
        protected abstract IDistributedCacheProvider[] CacheProviders { get; }

        #region Get
        protected override CacheEntry<string, TValue> GetCacheEntryInternal<TValue>(string key, string region)
        {
            throw new NotImplementedException();
        }

        protected override Task<CacheEntry<string, TValue>> GetCacheEntryInternalAsync<TValue>(string key, string region)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetOrAdd
        public TValue GetOrAdd<TValue>(string key, TValue value, CacheEntryOptions options)
        {
            return GetOrAdd(key, value, options, null);
        }

        public virtual TValue GetOrAdd<TValue>(string key, TValue value, CacheEntryOptions options, string region)
        {
            var cacheEntry = GetCacheEntry<TValue>(key, region);

            if (cacheEntry != null && string.Compare(cacheEntry.Key, key, true) == 0)
            {
                return cacheEntry.Value;
            }

            Set(key, value, options, region);
            return value;
        }

        public Task<TValue> GetOrAddAsync<TValue>(string key, TValue value, CacheEntryOptions options)
        {
            return GetOrAddAsync(key, value, options, null);
        }

        public virtual async Task<TValue> GetOrAddAsync<TValue>(string key, TValue value, CacheEntryOptions options, string region)
        {
            var cacheEntry = await GetCacheEntryAsync<TValue>(key, region).ConfigureAwait(false);

            if (cacheEntry != null && string.Compare(cacheEntry.Key, key, true) == 0)
            {
                return cacheEntry.Value;
            }

            await SetAsync(key, value, options, region).ConfigureAwait(false);
            return value;
        }
        #endregion

        #region Set
        protected override void SetInternal<TValue>(CacheEntry<string, TValue> entry)
        {
            foreach(var provider in CacheProviders)
            {
                provider.Set(entry);
            }
        }

        protected override Task SetInternalAsync<TValue>(CacheEntry<string, TValue> entry)
        {
            return Task.Run(() =>
            {
                foreach (var provider in CacheProviders)
                {
                    provider.SetAsync(entry).ConfigureAwait(false);
                }
            });
        }
        #endregion

        #region Remove
        protected override void RemoveInternal(string key, string region)
        {
            foreach (var provider in CacheProviders)
            {
                provider.Remove(key, region);
            }
        }

        protected override Task RemoveInternalAsync(string key, string region)
        {
            return Task.Run(() =>
            {
                foreach (var provider in CacheProviders)
                {
                    provider.RemoveAsync(key, region).ConfigureAwait(false);
                }
            });
        }
        #endregion

        #region Clear
        protected override void ClearInternal()
        {
            foreach (var provider in CacheProviders)
            {
                provider.Clear();
            }
        }

        protected override Task ClearInternalAsync()
        {
            return Task.Run(() =>
            {
                foreach (var provider in CacheProviders)
                {
                    provider.ClearAsync().ConfigureAwait(false);
                }
            });
        }

        protected override void ClearRegionInternal(string region)
        {
            foreach (var provider in CacheProviders)
            {
                provider.ClearRegion(region);
            }
        }

        protected override Task ClearRegionInternalAsync(string region)
        {
            return Task.Run(() =>
            {
                foreach (var provider in CacheProviders)
                {
                    provider.ClearRegionAsync(region).ConfigureAwait(false);
                }
            });
        }
        #endregion

        #region IDisposable Support
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach(var cacheProvider in CacheProviders)
                {
                    cacheProvider.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
