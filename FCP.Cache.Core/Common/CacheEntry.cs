namespace FCP.Cache
{
    /// <summary>
    /// 缓存实体
    /// </summary>
    public class CacheEntry<TKey, TValue>
    {
        public CacheEntry(TKey key, TValue value, CacheEntryOptions options)
        {
            Key = key;
            Value = value;
            Options = options;
        }

        public TKey Key { get; private set; }

        public TValue Value { get; private set; }

        public CacheEntryOptions Options { get; private set; }
    }
}
