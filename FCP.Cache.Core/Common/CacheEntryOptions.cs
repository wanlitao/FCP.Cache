using System;

namespace FCP.Cache
{
    /// <summary>
    /// 缓存实体 选项
    /// </summary>
    public class CacheEntryOptions
    {
        /// <summary>
        /// Get the create utc time
        /// </summary>
        public DateTime CreatedUtc { get; set; }

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

        /// <summary>
        /// Get the Valid State
        /// </summary>
        public bool IsValid
        {
            get
            {
                return !(ExpirationMode != ExpirationMode.None && ExpirationTimeout <= TimeSpan.Zero);
            }
        }
    }
}
