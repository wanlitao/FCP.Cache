using System;
using System.Threading.Tasks;

namespace FCP.Cache.Redis
{
    public class RedisCacheProvider : BaseDistributedCacheProvider
    {
        #region Get
        protected override CacheEntry<string, TValue> GetCacheEntryInternal<TValue>(string key, string region)
        {
            throw new NotImplementedException();
        }

        protected override async Task<CacheEntry<string, TValue>> GetCacheEntryInternalAsync<TValue>(string key, string region)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Set
        protected override void SetInternal<TValue>(CacheEntry<string, TValue> entry)
        {
            throw new NotImplementedException();
        }

        protected override async Task SetInternalAsync<TValue>(CacheEntry<string, TValue> entry)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Remove
        protected override void RemoveInternal(string key, string region)
        {
            throw new NotImplementedException();
        }

        protected override async Task RemoveInternalAsync(string key, string region)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Clear
        public override void Clear()
        {
            throw new NotImplementedException();
        }

        protected override void ClearRegionInternal(string region)
        {
            throw new NotImplementedException();
        }

        public override async Task ClearAsync()
        {
            throw new NotImplementedException();
        }

        protected override async Task ClearRegionInternalAsync(string region)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IDisposable Support
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // TODO: 释放托管状态(托管对象)。
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
