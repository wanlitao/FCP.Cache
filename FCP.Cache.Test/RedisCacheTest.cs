using System;
using Xunit;
using System.Threading;
using FCP.Cache.Redis;

namespace FCP.Cache.Test
{
    public class RedisCacheTest
    {
        protected const string redisConfiguration = "localhost";

        [Fact]
        public void RedisCache_Absolute_Expire()
        {
            var key = Guid.NewGuid().ToString("N");
            var options = CacheEntryOptionsFactory.AbSolute().Timeout(TimeSpan.FromMilliseconds(300));

            var redisCache = new RedisCacheProvider(redisConfiguration);
            redisCache.Set(key, "something", options);

            Assert.Equal("something", redisCache.Get<string>(key));

            Thread.Sleep(300);

            Assert.Null(redisCache.Get<string>(key));
        }
    }
}
