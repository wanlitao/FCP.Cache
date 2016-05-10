using System;

namespace FCP.Cache
{
    public abstract class BaseCacheProvider<TKey> : ICacheProvider<TKey>
    {
        #region Get
        public TValue Get<TValue>(TKey key)
        {
            return Get<TValue>(key, null);
        }

        public virtual TValue Get<TValue>(TKey key, string region)
        {
            var cacheEntry = GetCacheEntry<TValue>(key, region);

            if (cacheEntry != null && cacheEntry.Key.Equals(key))
            {
                return cacheEntry.Value;
            }
            return default(TValue);
        }

        public CacheEntry<TKey, TValue> GetCacheEntry<TValue>(TKey key)
        {
            return GetCacheEntry<TValue>(key, null);
        }

        public virtual CacheEntry<TKey, TValue> GetCacheEntry<TValue>(TKey key, string region)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            CheckDisposed();

            return GetCacheEntryInternal<TValue>(key, region);
        }

        protected abstract CacheEntry<TKey, TValue> GetCacheEntryInternal<TValue>(TKey key, string region);
        #endregion

        #region Set
        public void Set<TValue>(TKey key, TValue value, CacheEntryOptions options)
        {
            Set(key, value, options, null);
        }

        public virtual void Set<TValue>(TKey key, TValue value, CacheEntryOptions options, string region)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));            

            var cacheEntry = new CacheEntry<TKey, TValue>(key, region, value, options);

            Set(cacheEntry);
        }

        public virtual void Set<TValue>(CacheEntry<TKey, TValue> entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            CheckDisposed();
            
            SetInternal(entry);
        }

        protected abstract void SetInternal<TValue>(CacheEntry<TKey, TValue> entry);
        #endregion

        #region Remove
        public void Remove(TKey key)
        {
            Remove(key, null);
        }

        public virtual void Remove(TKey key, string region)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            CheckDisposed();

            RemoveInternal(key, region);
        }

        protected abstract void RemoveInternal(TKey key, string region);
        #endregion

        #region Clear
        public virtual void Clear()
        {
            CheckDisposed();

            ClearInternal();
        }

        protected abstract void ClearInternal();

        public virtual void ClearRegion(string region)
        {
            if (string.IsNullOrEmpty(region))
                throw new ArgumentNullException(nameof(region));

            CheckDisposed();

            ClearRegionInternal(region);
        }

        protected abstract void ClearRegionInternal(string region);
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected void CheckDisposed()
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }
                disposedValue = true;
            }
        }
        
        ~BaseCacheProvider() {
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
