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

            if (!valueItem.HasValue || valueItem.IsNull)  /* partially removed? */
                return null;

            var cacheEntry = new CacheEntry<string, TValue>(keyItem, regionItem,
                valueConverter.FromRedisValue<TValue>(valueItem),
                valueConverter.FromRedisValue<CacheEntryOptions>(optionsItem));

            return cacheEntry;
        }

        internal static CacheEntry<string, TValue> HashEntryGet<TValue>(this IDatabase cache, string key, IRedisValueConverter valueConverter)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));            

            var results = cache.HashGet(key, EntryHashFields);

            return convertCacheEntry<TValue>(results, valueConverter);
        }

        internal static async Task<CacheEntry<string, TValue>> HashEntryGetAsync<TValue>(this IDatabase cache, string key, IRedisValueConverter valueConverter)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var results = await cache.HashGetAsync(key, EntryHashFields).ConfigureAwait(false);

            return convertCacheEntry<TValue>(results, valueConverter);
        }
        #endregion

        #region Entry Set
        private static HashEntry[] convertHashEntry<TValue>(CacheEntry<string, TValue> entry, IRedisValueConverter valueConverter)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            if (valueConverter == null)
                throw new ArgumentNullException(nameof(valueConverter));

            var hashEntries = new HashEntry[]
            {
                new HashEntry(RedisCacheConstants.HashField_Key, entry.Key),
                new HashEntry(RedisCacheConstants.HashField_Region, entry.Region ?? string.Empty),
                new HashEntry(RedisCacheConstants.HashField_Value, valueConverter.ToRedisValue(entry.Value)),
                new HashEntry(RedisCacheConstants.HashField_Options, valueConverter.ToRedisValue(entry.Options))
            };

            return hashEntries;
        }

        internal static void HashEntrySet<TValue>(this IDatabase cache, string key, CacheEntry<string, TValue> entry, IRedisValueConverter valueConverter)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));            

            var hashEntries = convertHashEntry(entry, valueConverter);

            cache.HashSet(key, hashEntries, CommandFlags.FireAndForget);
        }

        internal static async Task HashEntrySetAsync<TValue>(this IDatabase cache, string key, CacheEntry<string, TValue> entry, IRedisValueConverter valueConverter)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var hashEntries = convertHashEntry(entry, valueConverter);

            await cache.HashSetAsync(key, hashEntries, CommandFlags.FireAndForget).ConfigureAwait(false);
        }
        #endregion
    }
}
