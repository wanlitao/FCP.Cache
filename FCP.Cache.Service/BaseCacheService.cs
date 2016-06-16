using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FCP.Cache.Service
{
    public abstract class BaseCacheService : BaseDistributedCacheProvider, ICacheService
    {
        protected readonly ConcurrentTaskManager _taskManager = new ConcurrentTaskManager();
        protected readonly ConcurrentTaskManager _valueTaskManager = new ConcurrentTaskManager();

        protected abstract IReadOnlyList<IDistributedCacheProvider> CacheProviders { get; }

        #region Cache Synchronize
        /// <summary>
        /// Get the cache entry to sync set
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="entry"></param>
        /// <returns></returns>
        protected CacheEntry<string, TValue> GetSyncSetCacheEntry<TValue>(CacheEntry<string, TValue> entry)
        {
            if (entry == null)
                return null;

            var setEntry = entry.Clone();

            var options = setEntry.Options;
            if (options.ExpirationMode == ExpirationMode.Absolute)
            {
                //calc new expiration timeout to make the entry timeout at the same time in all cache providers
                var newExpirationTimeout = options.CreatedUtc.Add(options.ExpirationTimeout) - DateTime.UtcNow;                
                options.Timeout(newExpirationTimeout);
            }

            return setEntry;
        }

        protected Task AddToBeforeCacheProvidersAsync<TValue>(CacheEntry<string, TValue> entry, int foundIndex)
        {
            return Task.Run(() =>
            {
                if (entry == null || foundIndex < 1)
                    return;

                var setEntry = GetSyncSetCacheEntry(entry);

                for (var providerIndex = 0; providerIndex < CacheProviders.Count && providerIndex < foundIndex; providerIndex++)
                {
                    CacheProviders[providerIndex].Set(setEntry);
                }
            });
        }

        protected Task AddToAfterCacheProvidersAsync<TValue>(CacheEntry<string, TValue> entry, int foundIndex)
        {
            return Task.Run(() =>
            {
                if (entry == null || foundIndex >= CacheProviders.Count - 1)
                    return;

                var setEntry = GetSyncSetCacheEntry(entry);

                for (var providerIndex = foundIndex + 1; providerIndex < CacheProviders.Count; providerIndex++)
                {
                    CacheProviders[providerIndex].Set(setEntry);
                }
            });
        }
        #endregion

        #region Get
        protected override CacheEntry<string, TValue> GetCacheEntryInternal<TValue>(string key, string region)
        {
            CacheEntry<string, TValue> entry = null;

            for(var providerIndex = 0; providerIndex < CacheProviders.Count; providerIndex++)
            {
                var provider = CacheProviders[providerIndex];
                entry = provider.GetCacheEntry<TValue>(key, region);

                if (entry != null)
                {
                    AddToBeforeCacheProvidersAsync(entry, providerIndex);  //sync the cache entry to before cache providers
                    break;
                }
            }

            return entry;
        }

        protected override async Task<CacheEntry<string, TValue>> GetCacheEntryInternalAsync<TValue>(string key, string region)
        {
            CacheEntry<string, TValue> entry = null;

            for (var providerIndex = 0; providerIndex < CacheProviders.Count; providerIndex++)
            {
                var provider = CacheProviders[providerIndex];
                entry = await provider.GetCacheEntryAsync<TValue>(key, region).ConfigureAwait(false);

                if (entry != null)
                {
                    AddToBeforeCacheProvidersAsync(entry, providerIndex);  //sync the cache entry to before cache providers
                    break;
                }
            }

            return entry;
        }
        #endregion

        #region GetOrAdd
        public TValue GetOrAdd<TValue>(string key, Func<string, TValue> valueFactory, CacheEntryOptions options)
        {
            return GetOrAdd(key, valueFactory, options, null);
        }

        public virtual TValue GetOrAdd<TValue>(string key, Func<string, TValue> valueFactory, CacheEntryOptions options, string region)
        {
            var cacheEntry = GetCacheEntry<TValue>(key, region);

            if (cacheEntry != null && string.Compare(cacheEntry.Key, key, true) == 0)
            {
                return cacheEntry.Value;
            }

            var setTask = _taskManager.GetOrAdd(key, () => SetByValueFactory(key, valueFactory, options, region)) as Task<TValue>;

            return setTask.Result;
        }

        public Task<TValue> GetOrAddAsync<TValue>(string key, Func<string, Task<TValue>> valueAsyncFactory, CacheEntryOptions options)
        {
            return GetOrAddAsync(key, valueAsyncFactory, options, null);
        }

        public virtual async Task<TValue> GetOrAddAsync<TValue>(string key, Func<string, Task<TValue>> valueAsyncFactory, CacheEntryOptions options, string region)
        {
            var cacheEntry = await GetCacheEntryAsync<TValue>(key, region).ConfigureAwait(false);

            if (cacheEntry != null && string.Compare(cacheEntry.Key, key, true) == 0)
            {
                return cacheEntry.Value;
            }

            var valueTask = _valueTaskManager.GetOrAdd(key, () => valueAsyncFactory(key)) as Task<TValue>;
            await valueTask.ConfigureAwait(false);                        

            var setTask = _taskManager.GetOrAdd(key, () => SetByValueFactory(key, (k) => { return valueTask.Result; }, options, region)) as Task<TValue>;
           
            return setTask.Result;
        }

        #region Set By ValueFactory
        protected Task<TValue> SetByValueFactory<TValue>(string key, Func<string, TValue> valueFactory, CacheEntryOptions options, string region)
        {
            var value = valueFactory(key);
            
            var entry = new CacheEntry<string, TValue>(key, region, value, options);
            if (CacheProviders.Count > 0)
            {
                CacheProviders[0].Set(entry);
                AddToAfterCacheProvidersAsync(entry, 0);
            }

            return Task.FromResult(value);
        }
        #endregion

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
            return Task.Run(async () =>
            {
                foreach (var provider in CacheProviders)
                {
                    await provider.SetAsync(entry).ConfigureAwait(false);
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
            return Task.Run(async () =>
            {
                foreach (var provider in CacheProviders)
                {
                    await provider.RemoveAsync(key, region).ConfigureAwait(false);
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
            return Task.Run(async () =>
            {
                foreach (var provider in CacheProviders)
                {
                    await provider.ClearAsync().ConfigureAwait(false);
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
            return Task.Run(async () =>
            {
                foreach (var provider in CacheProviders)
                {
                    await provider.ClearRegionAsync(region).ConfigureAwait(false);
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
