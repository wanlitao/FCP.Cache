using StackExchange.Redis;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FCP.Cache.Redis
{
    public class RedisCacheProvider : BaseDistributedCacheProvider
    {
        private readonly RedisConnection _connection;
        private readonly RedisValueConverter _valueConverter;

        #region Constructor
        public RedisCacheProvider(string configuration)
            : this(ParseConfigurationOptions(configuration))
        { }

        public RedisCacheProvider(ConfigurationOptions configOptions)
            : this(configOptions, new JsonCacheSerializer())
        { }

        public RedisCacheProvider(ConfigurationOptions configOptions, ICacheSerializer serializer)
        {
            if (configOptions == null)
                throw new ArgumentNullException(nameof(configOptions));

            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            _connection = new RedisConnection(configOptions);
            _valueConverter = new RedisValueConverter(serializer);
        }
        #endregion

        #region ConfigurationOptions
        /// <summary>
        /// 获取连接配置
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        protected static ConfigurationOptions ParseConfigurationOptions(string configuration)
        {
            if (string.IsNullOrEmpty(configuration))
                throw new ArgumentNullException(nameof(configuration));

            return ConfigurationOptions.Parse(configuration);
        }
        #endregion

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

        #region Servers
        public IEnumerable<IServer> Servers
        {
            get
            {
                var multiplexer = _connection.Connect();

                var endpoints = multiplexer.GetEndPoints();

                return endpoints.Select(m => multiplexer.GetServer(m));
            }
        }

        protected async Task<IEnumerable<IServer>> ServersAsync()
        {
            var multiplexer = await _connection.ConnectAsync().ConfigureAwait(false);

            var endpoints = multiplexer.GetEndPoints();

            return endpoints.Select(m => multiplexer.GetServer(m));
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

            var database = await DatabaseAsync().ConfigureAwait(false);
            var entry = await database.HashEntryGetAsync<TValue>(fullKey, _valueConverter).ConfigureAwait(false);

            if (entry != null && entry.Options.ExpirationMode == ExpirationMode.Sliding)
            {
                await database.KeyExpireAsync(fullKey, entry.Options.ExpirationTimeout, CommandFlags.FireAndForget).ConfigureAwait(false);
            }

            return entry;
        }
        #endregion

        #region Set
        protected override void SetInternal<TValue>(CacheEntry<string, TValue> entry)
        {
            var fullKey = GetEntryKey(entry.Key, entry.Region);

            entry.Options.CreatedUtc = DateTime.UtcNow;
            Database.HashEntrySet(fullKey, entry, _valueConverter);

            //update expire
            var entryOptions = entry.Options;
            var expireTimespan = entryOptions.ExpirationMode != ExpirationMode.None
                ? entryOptions.ExpirationTimeout : default(TimeSpan?);
            
            Database.KeyExpire(fullKey, expireTimespan, CommandFlags.FireAndForget);

            //update region lookup
            if (!string.IsNullOrEmpty(entry.Region))
            {
                Database.HashSet(entry.Region, fullKey, RedisCacheConstants.Region_HashField_Key_Val, When.Always, CommandFlags.FireAndForget);
            }
        }

        protected override async Task SetInternalAsync<TValue>(CacheEntry<string, TValue> entry)
        {
            var fullKey = GetEntryKey(entry.Key, entry.Region);
            var database = await DatabaseAsync().ConfigureAwait(false);

            entry.Options.CreatedUtc = DateTime.UtcNow;
            await database.HashEntrySetAsync(fullKey, entry, _valueConverter).ConfigureAwait(false);

            //update expire
            var entryOptions = entry.Options;
            var expireTimespan = entryOptions.ExpirationMode != ExpirationMode.None
                ? entryOptions.ExpirationTimeout : default(TimeSpan?);

            await database.KeyExpireAsync(fullKey, expireTimespan, CommandFlags.FireAndForget).ConfigureAwait(false);

            //update region lookup
            if (!string.IsNullOrEmpty(entry.Region))
            {
                await database.HashSetAsync(entry.Region, fullKey, RedisCacheConstants.Region_HashField_Key_Val,
                    When.Always, CommandFlags.FireAndForget).ConfigureAwait(false);
            }
        }
        #endregion

        #region Remove
        protected override void RemoveInternal(string key, string region)
        {
            var fullKey = GetEntryKey(key, region);

            if (!string.IsNullOrEmpty(region))
            {
                Database.HashDelete(region, fullKey, CommandFlags.FireAndForget);
            }

            Database.KeyDelete(fullKey, CommandFlags.FireAndForget);
        }

        protected override async Task RemoveInternalAsync(string key, string region)
        {
            var fullKey = GetEntryKey(key, region);

            var database = await DatabaseAsync().ConfigureAwait(false);

            if (!string.IsNullOrEmpty(region))
            {
                await database.HashDeleteAsync(region, fullKey, CommandFlags.FireAndForget).ConfigureAwait(false);
            }

            await database.KeyDeleteAsync(fullKey, CommandFlags.FireAndForget).ConfigureAwait(false);
        }
        #endregion

        #region Clear
        protected override void ClearInternal()
        {
            var databaseIndex = _connection.Configuration.DefaultDatabase ?? 0;

            foreach (var server in Servers.Where(p => !p.IsSlave))
            {
                if (server.IsConnected)
                {
                    server.FlushDatabase(databaseIndex, CommandFlags.FireAndForget);
                }
            }
        }

        protected override async Task ClearInternalAsync()
        {
            var servers = await ServersAsync().ConfigureAwait(false);
            var databaseIndex = _connection.Configuration.DefaultDatabase ?? 0;

            foreach (var server in servers.Where(p => !p.IsSlave))
            {
                if (server.IsConnected)
                {
                    await server.FlushDatabaseAsync(databaseIndex, CommandFlags.FireAndForget).ConfigureAwait(false);
                }
            }            
        }

        protected override void ClearRegionInternal(string region)
        {
            var hashKeys = Database.HashKeys(region);

            if (hashKeys != null && hashKeys.Length > 0)
            {
                // lets remove all keys which where in the region
                // 01/32/16 changed to remove one by one because on clusters the keys could belong to multiple slots
                foreach (var key in hashKeys.Where(p => p.HasValue))
                {
                    Database.KeyDelete(key.ToString(), CommandFlags.FireAndForget);
                }                
            }

            // now delete the region
            Database.KeyDelete(region, CommandFlags.FireAndForget);
        }
        
        protected override async Task ClearRegionInternalAsync(string region)
        {
            var database = await DatabaseAsync().ConfigureAwait(false);

            var hashKeys = await database.HashKeysAsync(region).ConfigureAwait(false);

            if (hashKeys != null && hashKeys.Length > 0)
            {
                // lets remove all keys which where in the region
                // 01/32/16 changed to remove one by one because on clusters the keys could belong to multiple slots
                foreach (var key in hashKeys.Where(p => p.HasValue))
                {
                    await database.KeyDeleteAsync(key.ToString(), CommandFlags.FireAndForget).ConfigureAwait(false);
                }
            }

            // now delete the region
            await database.KeyDeleteAsync(region, CommandFlags.FireAndForget).ConfigureAwait(false);           
        }
        #endregion

        #region IDisposable Support
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connection.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
