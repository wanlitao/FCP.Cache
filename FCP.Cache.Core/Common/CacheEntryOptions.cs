using System;

namespace FCP.Cache
{
    /// <summary>
    /// 缓存实体 选项
    /// </summary>
    public class CacheEntryOptions
    {
        /// <summary>
        /// Gets the expiration mode.
        /// </summary>
        /// <value>The expiration mode.</value>
        public ExpirationMode ExpirationMode { get; set; }

        /// <summary>
        /// Gets the expiration timeout.
        /// </summary>
        /// <value>The expiration timeout.</value>
        public TimeSpan ExpirationTimeout { get; set; }
    }
}
