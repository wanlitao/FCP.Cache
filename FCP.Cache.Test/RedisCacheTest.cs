using System;
using Xunit;
using System.Threading;
using FCP.Cache.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCP.Cache.Test
{
    public class RedisCacheTest
    {
        protected const string redisConfiguration = "localhost,allowAdmin=true";

        #region sync test
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

        [Fact]
        public void RedisCache_Sliding_Expire()
        {
            var redisCache = new RedisCacheProvider(redisConfiguration);

            var key = Guid.NewGuid().ToString("N");
            var options = CacheEntryOptionsFactory.Sliding().Timeout(TimeSpan.FromMilliseconds(300));
            redisCache.Set(key, "something", options);

            Thread.Sleep(250);
            Assert.Equal("something", redisCache.Get<string>(key));

            Thread.Sleep(250);
            Assert.Equal("something", redisCache.Get<string>(key));

            Thread.Sleep(350);
            Assert.Null(redisCache.Get<string>(key));

            options = CacheEntryOptionsFactory.Sliding().Timeout(TimeSpan.FromSeconds(2));
            redisCache.Set(key, "something", options);

            Thread.Sleep(1800);
            Assert.Equal("something", redisCache.Get<string>(key));

            Thread.Sleep(1800);
            Assert.Equal("something", redisCache.Get<string>(key));

            Thread.Sleep(2200);
            Assert.Null(redisCache.Get<string>(key));
        }

        [Fact]
        public void RedisCache_Remove()
        {
            var key = Guid.NewGuid().ToString("N");

            using (var redisCache = new RedisCacheProvider(redisConfiguration))
            {
                redisCache.Set(key, "something");
                Assert.Equal("something", redisCache.Get<string>(key));

                redisCache.Remove(key);
                Assert.Null(redisCache.Get<string>(key));
            }
        }

        [Fact]
        public void RedisCache_Clear()
        {
            var redisCache = new RedisCacheProvider(redisConfiguration);

            var keys = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var key = Guid.NewGuid().ToString("N");
                redisCache.Set(key, "something");
                keys.Add(key);
            }
            Assert.Equal("something", redisCache.Get<string>(keys.First()));
            Assert.Equal("something", redisCache.Get<string>(keys.Last()));

            redisCache.Clear();
            foreach (var key in keys)
            {
                Assert.Null(redisCache.Get<string>(key));
            }
        }

        [Fact]
        public void RedisCache_Region()
        {
            var redisCache = new RedisCacheProvider(redisConfiguration);

            var keys = new List<string>();
            var regionKeys = new List<string>();
            var region = "test";
            var value = "something";

            for (int i = 0; i < 10; i++)
            {
                var regionKey = Guid.NewGuid().ToString("N");
                redisCache.Set(regionKey, value, region);
                regionKeys.Add(regionKey);
            }
            Assert.Equal(value, redisCache.Get<string>(regionKeys.First(), region));
            Assert.Equal(value, redisCache.Get<string>(regionKeys.Last(), region));

            redisCache.Remove(regionKeys.First(), region);
            Assert.Null(redisCache.Get<string>(regionKeys.First(), region));

            for (int i = 0; i < 10; i++)
            {
                var key = Guid.NewGuid().ToString("N");
                redisCache.Set(key, value);
                keys.Add(key);
            }
            Assert.Equal(value, redisCache.Get<string>(keys.First()));
            Assert.Equal(value, redisCache.Get<string>(keys.Last()));
            Assert.Null(redisCache.Get<string>(keys.First(), region));

            redisCache.ClearRegion(region);
            foreach (var regionKey in regionKeys)
            {
                Assert.Null(redisCache.Get<string>(regionKey, region));
            }
            foreach (var key in keys)
            {
                Assert.Equal(value, redisCache.Get<string>(key));
            }
        }
        #endregion

        #region async test
        [Fact]
        public async Task RedisCache_Absolute_Expire_Async()
        {
            var key = Guid.NewGuid().ToString("N");
            var options = CacheEntryOptionsFactory.AbSolute().Timeout(TimeSpan.FromMilliseconds(300));

            var redisCache = new RedisCacheProvider(redisConfiguration);
            await redisCache.SetAsync(key, "something", options).ConfigureAwait(false);

            Assert.Equal("something", await redisCache.GetAsync<string>(key).ConfigureAwait(false));

            Thread.Sleep(350);

            Assert.Null(await redisCache.GetAsync<string>(key).ConfigureAwait(false));
        }

        [Fact]
        public async Task RedisCache_Sliding_Expire_Async()
        {
            var redisCache = new RedisCacheProvider(redisConfiguration);

            var key = Guid.NewGuid().ToString("N");
            var options = CacheEntryOptionsFactory.Sliding().Timeout(TimeSpan.FromMilliseconds(1000));
            await redisCache.SetAsync(key, "something", options).ConfigureAwait(false);

            Thread.Sleep(800);
            Assert.Equal("something", await redisCache.GetAsync<string>(key).ConfigureAwait(false));

            Thread.Sleep(800);
            Assert.Equal("something", await redisCache.GetAsync<string>(key).ConfigureAwait(false));

            Thread.Sleep(1200);
            Assert.Null(await redisCache.GetAsync<string>(key).ConfigureAwait(false));

            options = CacheEntryOptionsFactory.Sliding().Timeout(TimeSpan.FromSeconds(2));
            await redisCache.SetAsync(key, "something", options).ConfigureAwait(false);

            Thread.Sleep(1800);
            Assert.Equal("something", await redisCache.GetAsync<string>(key).ConfigureAwait(false));

            Thread.Sleep(1800);
            Assert.Equal("something", await redisCache.GetAsync<string>(key).ConfigureAwait(false));

            Thread.Sleep(2200);
            Assert.Null(await redisCache.GetAsync<string>(key).ConfigureAwait(false));
        }

        [Fact]
        public async Task RedisCache_Remove_Async()
        {
            var key = Guid.NewGuid().ToString("N");

            using (var redisCache = new RedisCacheProvider(redisConfiguration))
            {
                await redisCache.SetAsync(key, "something").ConfigureAwait(false);
                Assert.Equal("something", await redisCache.GetAsync<string>(key).ConfigureAwait(false));

                await redisCache.RemoveAsync(key).ConfigureAwait(false);
                Assert.Null(await redisCache.GetAsync<string>(key).ConfigureAwait(false));
            }
        }

        [Fact]
        public async Task RedisCache_Clear_Async()
        {
            var redisCache = new RedisCacheProvider(redisConfiguration);

            var keys = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var key = Guid.NewGuid().ToString("N");
                await redisCache.SetAsync(key, "something").ConfigureAwait(false);
                keys.Add(key);
            }
            Assert.Equal("something", await redisCache.GetAsync<string>(keys.First()).ConfigureAwait(false));
            Assert.Equal("something", await redisCache.GetAsync<string>(keys.Last()).ConfigureAwait(false));

            await redisCache.ClearAsync().ConfigureAwait(false);
            foreach (var key in keys)
            {
                Assert.Null(await redisCache.GetAsync<string>(key).ConfigureAwait(false));
            }
        }

        [Fact]
        public async Task RedisCache_Region_Async()
        {
            var redisCache = new RedisCacheProvider(redisConfiguration);

            var keys = new List<string>();
            var regionKeys = new List<string>();
            var region = "test";
            var value = "something";

            for (int i = 0; i < 10; i++)
            {
                var regionKey = Guid.NewGuid().ToString("N");
                await redisCache.SetAsync(regionKey, value, region).ConfigureAwait(false);
                regionKeys.Add(regionKey);
            }
            Assert.Equal(value, await redisCache.GetAsync<string>(regionKeys.First(), region).ConfigureAwait(false));
            Assert.Equal(value, await redisCache.GetAsync<string>(regionKeys.Last(), region).ConfigureAwait(false));

            await redisCache.RemoveAsync(regionKeys.First(), region).ConfigureAwait(false);
            Assert.Null(await redisCache.GetAsync<string>(regionKeys.First(), region).ConfigureAwait(false));

            for (int i = 0; i < 10; i++)
            {
                var key = Guid.NewGuid().ToString("N");
                await redisCache.SetAsync(key, value).ConfigureAwait(false);
                keys.Add(key);
            }
            Assert.Equal(value, await redisCache.GetAsync<string>(keys.First()).ConfigureAwait(false));
            Assert.Equal(value, await redisCache.GetAsync<string>(keys.Last()).ConfigureAwait(false));
            Assert.Null(await redisCache.GetAsync<string>(keys.First(), region).ConfigureAwait(false));

            await redisCache.ClearRegionAsync(region).ConfigureAwait(false);
            foreach (var regionKey in regionKeys)
            {
                Assert.Null(await redisCache.GetAsync<string>(regionKey, region).ConfigureAwait(false));
            }
            foreach (var key in keys)
            {
                Assert.Equal(value, await redisCache.GetAsync<string>(key).ConfigureAwait(false));
            }
        }
        #endregion
    }
}
