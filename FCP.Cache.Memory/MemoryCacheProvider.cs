using System;
using System.Runtime.Caching;
using System.Collections.Generic;

namespace FCP.Cache.Memory
{
    public class MemoryCacheProvider : BaseCacheProvider<string>
    {
        private const string defaultName = "default";
        public static MemoryCacheProvider Default = new MemoryCacheProvider();

        private readonly string _cacheName = string.Empty;

        private volatile MemoryCache _cache = null;
        private string _instanceKey = null;
        
        private MemoryCacheProvider()
        {
            _cacheName = defaultName;
            _cache = MemoryCache.Default;
            Init();
        }

        public MemoryCacheProvider(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (string.Compare(name, defaultName, true) == 0)
                throw new ArgumentException("default memory cache provider is reserved", nameof(name));

            _cacheName = name;
            _cache = new MemoryCache(name);
            Init();
        }

        private void Init()
        {
            ResetInstanceToken();
        }

        #region Token
        private void ResetInstanceToken()
        {
            _instanceKey = Guid.NewGuid().ToString("N");
            
            CacheItemPolicy tokenPolicy = new CacheItemPolicy()
            {
                Priority = CacheItemPriority.Default,
                RemovedCallback = new CacheEntryRemovedCallback(OnInstanceTokenRemoved),
                AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration,
                SlidingExpiration = ObjectCache.NoSlidingExpiration
            };

            _cache.Set(_instanceKey, _instanceKey, tokenPolicy);
        }

        protected void OnInstanceTokenRemoved(CacheEntryRemovedArguments arguments)
        {
            ResetInstanceToken();
        }

        protected void CheckRegionToken(string region)
        {
            var regionKey = GetRegionKey(region);

            if (!_cache.Contains(regionKey))
            {
                SetRegionToken(region);
            }
        }

        protected void SetRegionToken(string region)
        {
            var regionKey = GetRegionKey(region);

            CacheItemPolicy tokenPolicy = new CacheItemPolicy()
            {
                Priority = CacheItemPriority.Default,
                ChangeMonitors = { _cache.CreateCacheEntryChangeMonitor(new[] { _instanceKey }) },
                AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration,
                SlidingExpiration = ObjectCache.NoSlidingExpiration
            };

            _cache.Set(regionKey, region, tokenPolicy);
        }
        #endregion

        #region Key
        protected string GetRegionKey(string region)
        {
            if (string.IsNullOrEmpty(region))
                throw new ArgumentNullException(nameof(region));

            return string.Format("{0}@@{1}", _instanceKey, region);
        }

        protected string GetEntryKey(string key, string region)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (string.IsNullOrEmpty(region))
            {
                return string.Format("{0}::{1}", _instanceKey, key);
            }

            return string.Format("{0}@@{1}::{2}", _instanceKey, region, key);
        }
        #endregion

        #region Get
        protected override CacheEntry<string, TValue> GetCacheEntryInternal<TValue>(string key, string region)
        {
            var fullKey = GetEntryKey(key, region);
            var entry = _cache.Get(fullKey) as CacheEntry<string, TValue>;

            if (entry != null && entry.Options.ExpirationMode == ExpirationMode.Sliding)
            {
                // if sliding expire timeout is smaller than CacheExpires.MIN_UPDATE_DELTA (1s), will not update the entry expire time
                // so to avoid this issue reset the entry
                if (entry.Options.ExpirationTimeout <= TimeSpan.FromSeconds(1))
                {
                    var newCachePolicy = BuildCachePolicy(entry);
                    _cache.Set(fullKey, entry, newCachePolicy);
                }                                
            }

            return entry;
        }
        #endregion

        #region Set
        protected override void SetInternal<TValue>(CacheEntry<string, TValue> entry)
        {
            if (entry.IsInvalid)
                return;

            var fullKey = GetEntryKey(entry.Key, entry.Region);
            var cachePolicy = BuildCachePolicy(entry);

            entry.Options.CreatedUtc = DateTime.UtcNow;

            _cache.Set(fullKey, entry, cachePolicy);
        }
       
        protected CacheItemPolicy BuildCachePolicy<TValue>(CacheEntry<string, TValue> entry)
        {
            var monitorKeys = GetMonitorKeys(entry.Region);

            var cachePolicy = new CacheItemPolicy()
            {
                Priority = CacheItemPriority.Default,
                ChangeMonitors = { _cache.CreateCacheEntryChangeMonitor(monitorKeys) },
                AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration,
                SlidingExpiration = ObjectCache.NoSlidingExpiration,
            };

            var entryOptions = entry.Options;
            if (entryOptions.ExpirationMode == ExpirationMode.Absolute)
            {
                cachePolicy.AbsoluteExpiration = new DateTimeOffset(DateTime.UtcNow.Add(entryOptions.ExpirationTimeout));
            }
            else if (entryOptions.ExpirationMode == ExpirationMode.Sliding)
            {
                cachePolicy.SlidingExpiration = entryOptions.ExpirationTimeout;
            }

            return cachePolicy;
        }

        /// <summary>
        /// implement regions and RemoveAll/Clear via cache dependencies.
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        protected IEnumerable<string> GetMonitorKeys(string region)
        {
            var monitorKeys = new List<string> { _instanceKey };            

            if (!string.IsNullOrEmpty(region))
            {
                CheckRegionToken(region);  //check exists region token

                var regionKey = GetRegionKey(region);
                monitorKeys.Add(regionKey);
            }

            return monitorKeys;
        }
        #endregion

        #region Remove
        protected override void RemoveInternal(string key, string region)
        {
            var fullKey = GetEntryKey(key, region);

            _cache.Remove(fullKey);
        }
        #endregion        

        #region Clear
        protected override void ClearInternal()
        {
            _cache.Remove(_instanceKey);
        }

        protected override void ClearRegionInternal(string region)
        {
            var regionKey = GetRegionKey(region);

            _cache.Remove(regionKey);
        }
        #endregion

        #region IDisposable Support
        protected override void DisposeInternal()
        {
            _cache.Dispose();
            base.DisposeInternal();
        }
        #endregion
    }
}
