using System;
using System.Threading.Tasks;

namespace FCP.Cache
{
    public abstract class BaseDistributedCacheProvider : BaseCacheProvider<string>, IDistributedCacheProvider
    {
        public override TValue Get<TValue>(string key, string region)
        {
            throw new NotImplementedException();
        }

        public Task<TValue> GetAsync<TValue>(string key)
        {
            return GetAsync<TValue>(key, null);
        }

        public virtual Task<TValue> GetAsync<TValue>(string key, string region)
        {
            throw new NotImplementedException();
        }

        public override void Set<TValue>(string key, TValue value, CacheEntryOptions options, string region)
        {
            throw new NotImplementedException();
        }

        public Task SetAsync<TValue>(string key, TValue value, CacheEntryOptions options)
        {
            return SetAsync<TValue>(key, value, options, null);
        }

        public virtual Task SetAsync<TValue>(string key, TValue value, CacheEntryOptions options, string region)
        {
            throw new NotImplementedException();
        }        

        public override void Remove(string key, string region)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(string key)
        {
            return RemoveAsync(key, null);
        }

        public virtual Task RemoveAsync(string key, string region)
        {
            throw new NotImplementedException();
        }        

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public virtual Task ClearAsync()
        {
            throw new NotImplementedException();
        }

        public override void ClearRegion(string region)
        {
            throw new NotImplementedException();
        }

        public virtual Task ClearRegionAsync(string region)
        {
            throw new NotImplementedException();
        }
    }
}
