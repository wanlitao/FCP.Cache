using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace FCP.Cache.Redis
{
    internal static class RedisCacheExtensions
    {
        private static RedisValue[] EntryHashFields = new RedisValue[]
        {
            RedisCacheConstants.HashField_Key,
            RedisCacheConstants.HashField_Region,
            RedisCacheConstants.HashField_Value,
            RedisCacheConstants.HashField_Options
        };

        #region Entry Get
        private static CacheEntry<string, TValue> convertCacheEntry<TValue>(RedisValue[] redisValues, IRedisValueConverter valueConverter)
        {
            if (redisValues == null || redisValues.Length < 4)
                return null;

            if (valueConverter == null)
                throw new ArgumentNullException(nameof(valueConverter));

            var keyItem = redisValues[0];
            var regionItem = redisValues[1];
            var valueItem = redisValues[2];
            var optionsItem = redisValues[3];

            var cacheEntry = new CacheEntry<string, TValue>(keyItem, regionItem,
                valueConverter.FromRedisValue<TValue>(valueItem),
                valueConverter.FromRedisValue<CacheEntryOptions>(optionsItem));

            return cacheEntry;
        }

        public static CacheEntry<string, TValue> HashEntryGet<TValue>(this IDatabase cache, string key, IRedisValueConverter valueConverter)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));            

            var results = cache.HashGet(key, EntryHashFields);

            return convertCacheEntry<TValue>(results, valueConverter);
        }

        public static async Task<CacheEntry<string, TValue>> HashEntryGetAsync<TValue>(this IDatabase cache, string key, IRedisValueConverter valueConverter)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var results = await cache.HashGetAsync(key, EntryHashFields).ConfigureAwait(false);

            return convertCacheEntry<TValue>(results, valueConverter);
        }
        #endregion
    }
}
