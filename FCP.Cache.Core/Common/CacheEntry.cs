using System;

namespace FCP.Cache
{
    /// <summary>
    /// 缓存实体
    /// </summary>
    public class CacheEntry<TKey, TValue>
    {
        public CacheEntry(TKey key, TValue value)
            : this(key, value, null)
        { }

        public CacheEntry(TKey key, TValue value, string region)
            : this(key, region, value, new CacheEntryOptions())
        { }

        public CacheEntry(TKey key, string region, TValue value, CacheEntryOptions options)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            Key = key;
            Region = region;
            Value = value;
            Options = options;            
        }

        public TKey Key { get; private set; }

        public TValue Value { get; private set; }

        public CacheEntryOptions Options { get; private set; }

        public string Region { get; private set; }
        
        /// <summary>
        /// Get Clone Entry
        /// </summary>
        /// <returns></returns>
        public CacheEntry<TKey, TValue> Clone()
        {
            return new CacheEntry<TKey, TValue>(Key, Region, Value, Options.Clone());
        }        
    }
}
