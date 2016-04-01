using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace FCP.Cache.Redis
{
    public class RedisCacheProvider : BaseDistributedCacheProvider
    {
        private readonly RedisConnection _connection;
        private readonly RedisValueConverter _valueConverter;

        public RedisCacheProvider(string configuration)
            : this(configuration, new JsonCacheSerializer())
        { }

        public RedisCacheProvider(string configuration, ICacheSerializer serializer)
        {
            if (string.IsNullOrEmpty(configuration))
                throw new ArgumentNullException(nameof(configuration));

            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            _connection = new RedisConnection(configuration);
            _valueConverter = new RedisValueConverter(serializer);
        }

        #region Key
        protected string GetEntryKey(string key, string region)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            if (string.IsNullOrEmpty(region))
                return key;

            return string.Format("{0}::{1}", region, key);
        }
        #endregion

        #region Database
        protected IDatabase Database
        {
            get { return _connection.Connect().GetDatabase(); }            
        }

        protected async Task<IDatabase> DatabaseAsync()
        {
            var multiplexer = await _connection.ConnectAsync().ConfigureAwait(false);

            return multiplexer.GetDatabase();
        }
        #endregion

        #region Get
        protected override CacheEntry<string, TValue> GetCacheEntryInternal<TValue>(string key, string region)
        {
            var fullKey = GetEntryKey(key, region);
            var entry = Database.HashEntryGet<TValue>(fullKey, _valueConverter);

            if (entry != null && entry.Options.ExpirationMode == ExpirationMode.Sliding)
            {                
                Database.KeyExpire(fullKey, entry.Options.ExpirationTimeout, CommandFlags.FireAndForget);
            }

            return entry;
        }

        protected override async Task<CacheEntry<string, TValue>> GetCacheEntryInternalAsync<TValue>(string key, string region)
        {
            var fullKey = GetEntryKey(key, region);

            var datebase = await DatabaseAsync().ConfigureAwait(false);
            var entry = await datebase.HashEntryGetAsync<TValue>(fullKey, _valueConverter).ConfigureAwait(false);

            if (entry != null && entry.Options.ExpirationMode == ExpirationMode.Sliding)
            {
                await Database.KeyExpireAsync(fullKey, entry.Options.ExpirationTimeout, CommandFlags.FireAndForget).ConfigureAwait(false);
            }

            return entry;
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
        protected override void ClearInternal()
        {
            throw new NotImplementedException();
        }

        protected override async Task ClearInternalAsync()
        {
            throw new NotImplementedException();
        }

        protected override void ClearRegionInternal(string region)
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
