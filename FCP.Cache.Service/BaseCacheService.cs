using System;

namespace FCP.Cache.Service
{
    public abstract class BaseCacheService<TKey> : ICacheService<TKey>
    {
        #region Get
        public TValue Get<TValue>(TKey key)
        {
            return Get<TValue>(key, null);
        }

        public virtual TValue Get<TValue>(TKey key, string region)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetOrAdd
        public TValue GetOrAdd<TValue>(TKey key, TValue value, CacheEntryOptions options)
        {
            return GetOrAdd(key, value, options, null);
        }

        public virtual TValue GetOrAdd<TValue>(TKey key, TValue value, CacheEntryOptions options, string region)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Set
        public void Set<TValue>(TKey key, TValue value, CacheEntryOptions options)
        {
            Set(key, value, options, null);
        }

        public virtual void Set<TValue>(TKey key, TValue value, CacheEntryOptions options, string region)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Remove
        public void Remove(TKey key)
        {
            Remove(key, null);
        }

        public virtual void Remove(TKey key, string region)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Clear
        public virtual void Clear()
        {
            throw new NotImplementedException();
        }

        public virtual void ClearRegion(string region)
        {
            throw new NotImplementedException();
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
                    // TODO: 释放托管状态(托管对象)。
                }
                disposedValue = true;
            }
        }
        
        ~BaseCacheService() {
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
