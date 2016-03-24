using System;

namespace FCP.Cache
{
    public static class CacheEntryOptionsFactory
    {
        public static CacheEntryOptions Create()
        {
            return new CacheEntryOptions { ExpirationMode = ExpirationMode.None, ExpirationTimeout = TimeSpan.Zero };
        }

        public static CacheEntryOptions AbSolute()
        {
            var options = Create();

            options.ExpirationMode = ExpirationMode.Absolute;            

            return options;
        }

        public static CacheEntryOptions Sliding()
        {
            var options = Create();

            options.ExpirationMode = ExpirationMode.Sliding;           

            return options;
        }

        public static CacheEntryOptions Timeout(this CacheEntryOptions options, TimeSpan timeout)
        {
            if (timeout < TimeSpan.Zero)
                timeout = TimeSpan.Zero;
                      
            options.ExpirationTimeout = timeout;

            return options;
        }
    }
}
