using System;
using System.Runtime.Caching;

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
        protected void ResetInstanceToken()
        {
            _instanceKey = Guid.NewGuid().ToString("N");
            
            CacheItemPolicy tokenPolicy = new CacheItemPolicy()
            {
                Priority = CacheItemPriority.Default,
                RemovedCallback = new CacheEntryRemovedCallback(OnInstanceTokenRemoved),
                AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration,
                SlidingExpiration = ObjectCache.NoSlidingExpiration,
            };

            _cache.Add(_instanceKey, _instanceKey, tokenPolicy);
        }

        protected void OnInstanceTokenRemoved(CacheEntryRemovedArguments arguments)
        {
            ResetInstanceToken();
        }
        #endregion

        #region Key
        protected string GetRegionKey(string region)
        {
            if (string.IsNullOrEmpty(region))
                return string.Empty;

            return string.Format("{0}@{1}", _instanceKey, region);
        }

        protected string GetEntryKey(string key, string region)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (string.IsNullOrEmpty(region))
            {
                return string.Format("{0}:{1}", _instanceKey, key);
            }

            return string.Format("{0}@{1}:{2}", _instanceKey, region, key);
        }
        #endregion

        #region Get
        protected override CacheEntry<string, TValue> GetCacheEntryInternal<TValue>(string key, string region)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Set
        protected override void SetInternal<TValue>(CacheEntry<string, TValue> entry)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Remove
        protected override void RemoveInternal(string key, string region)
        {
            throw new NotImplementedException();
        }
        #endregion        

        #region Clear
        public override void Clear()
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
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cache.Dispose();                
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
