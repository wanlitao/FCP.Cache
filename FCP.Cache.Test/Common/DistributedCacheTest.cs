using System;
using Xunit;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCP.Cache.Test
{
    public abstract class DistributedCacheTest
    {
        protected abstract IDistributedCacheProvider GetDistributedCache();

        #region sync test
        [Fact]
        public void DistributedCache_Absolute_Expire()
        {
            var key = Guid.NewGuid().ToString("N");
            var options = CacheEntryOptionsFactory.AbSolute().Timeout(TimeSpan.FromMilliseconds(300));

            var distributedCache = GetDistributedCache();
            distributedCache.Set(key, "something", options);

            Assert.Equal("something", distributedCache.Get<string>(key));

            Thread.Sleep(320);

            Assert.Null(distributedCache.Get<string>(key));
        }

        [Fact]
        public void DistributedCache_Sliding_Expire()
        {
            var distributedCache = GetDistributedCache();

            var key = Guid.NewGuid().ToString("N");
            var options = CacheEntryOptionsFactory.Sliding().Timeout(TimeSpan.FromMilliseconds(500));
            distributedCache.Set(key, "something", options);

            Thread.Sleep(400);
            Assert.Equal("something", distributedCache.Get<string>(key));

            Thread.Sleep(400);
            Assert.Equal("something", distributedCache.Get<string>(key));

            Thread.Sleep(600);
            Assert.Null(distributedCache.Get<string>(key));

            options = CacheEntryOptionsFactory.Sliding().Timeout(TimeSpan.FromSeconds(2));
            distributedCache.Set(key, "something", options);

            Thread.Sleep(1800);
            Assert.Equal("something", distributedCache.Get<string>(key));

            Thread.Sleep(1800);
            Assert.Equal("something", distributedCache.Get<string>(key));

            Thread.Sleep(2200);
            Assert.Null(distributedCache.Get<string>(key));
        }

        [Fact]
        public void DistributedCache_Remove()
        {
            var key = Guid.NewGuid().ToString("N");

            using (var distributedCache = GetDistributedCache())
            {
                distributedCache.Set(key, "something");
                Assert.Equal("something", distributedCache.Get<string>(key));

                distributedCache.Remove(key);
                Assert.Null(distributedCache.Get<string>(key));
            }
        }

        [Fact]
        public void DistributedCache_Clear()
        {
            var distributedCache = GetDistributedCache();

            var keys = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var key = Guid.NewGuid().ToString("N");
                distributedCache.Set(key, "something");
                keys.Add(key);
            }
            Assert.Equal("something", distributedCache.Get<string>(keys.First()));
            Assert.Equal("something", distributedCache.Get<string>(keys.Last()));

            distributedCache.Clear();
            foreach (var key in keys)
            {
                Assert.Null(distributedCache.Get<string>(key));
            }
        }

        [Fact]
        public void DistributedCache_Region()
        {
            var distributedCache = GetDistributedCache();

            var keys = new List<string>();
            var regionKeys = new List<string>();
            var region = "test";
            var value = "something";

            for (int i = 0; i < 10; i++)
            {
                var regionKey = Guid.NewGuid().ToString("N");
                distributedCache.Set(regionKey, value, region);
                regionKeys.Add(regionKey);
            }
            Assert.Equal(value, distributedCache.Get<string>(regionKeys.First(), region));
            Assert.Equal(value, distributedCache.Get<string>(regionKeys.Last(), region));

            distributedCache.Remove(regionKeys.First(), region);
            Assert.Null(distributedCache.Get<string>(regionKeys.First(), region));

            for (int i = 0; i < 10; i++)
            {
                var key = Guid.NewGuid().ToString("N");
                distributedCache.Set(key, value);
                keys.Add(key);
            }
            Assert.Equal(value, distributedCache.Get<string>(keys.First()));
            Assert.Equal(value, distributedCache.Get<string>(keys.Last()));
            Assert.Null(distributedCache.Get<string>(keys.First(), region));

            distributedCache.ClearRegion(region);
            foreach (var regionKey in regionKeys)
            {
                Assert.Null(distributedCache.Get<string>(regionKey, region));
            }
            foreach (var key in keys)
            {
                Assert.Equal(value, distributedCache.Get<string>(key));
            }
        }
        #endregion

        #region async test
        [Fact]
        public async Task DistributedCache_Absolute_Expire_Async()
        {
            var key = Guid.NewGuid().ToString("N");
            var options = CacheEntryOptionsFactory.AbSolute().Timeout(TimeSpan.FromMilliseconds(300));

            var distributedCache = GetDistributedCache();
            await distributedCache.SetAsync(key, "something", options).ConfigureAwait(false);

            Assert.Equal("something", await distributedCache.GetAsync<string>(key).ConfigureAwait(false));

            Thread.Sleep(350);

            Assert.Null(await distributedCache.GetAsync<string>(key).ConfigureAwait(false));
        }

        [Fact]
        public async Task DistributedCache_Sliding_Expire_Async()
        {
            var distributedCache = GetDistributedCache();

            var key = Guid.NewGuid().ToString("N");
            var options = CacheEntryOptionsFactory.Sliding().Timeout(TimeSpan.FromMilliseconds(1000));
            await distributedCache.SetAsync(key, "something", options).ConfigureAwait(false);

            Thread.Sleep(800);
            Assert.Equal("something", await distributedCache.GetAsync<string>(key).ConfigureAwait(false));

            Thread.Sleep(800);
            Assert.Equal("something", await distributedCache.GetAsync<string>(key).ConfigureAwait(false));

            Thread.Sleep(1200);
            Assert.Null(await distributedCache.GetAsync<string>(key).ConfigureAwait(false));

            options = CacheEntryOptionsFactory.Sliding().Timeout(TimeSpan.FromSeconds(2));
            await distributedCache.SetAsync(key, "something", options).ConfigureAwait(false);

            Thread.Sleep(1800);
            Assert.Equal("something", await distributedCache.GetAsync<string>(key).ConfigureAwait(false));

            Thread.Sleep(1800);
            Assert.Equal("something", await distributedCache.GetAsync<string>(key).ConfigureAwait(false));

            Thread.Sleep(2200);
            Assert.Null(await distributedCache.GetAsync<string>(key).ConfigureAwait(false));
        }

        [Fact]
        public async Task DistributedCache_Remove_Async()
        {
            var key = Guid.NewGuid().ToString("N");

            using (var distributedCache = GetDistributedCache())
            {
                await distributedCache.SetAsync(key, "something").ConfigureAwait(false);
                Assert.Equal("something", await distributedCache.GetAsync<string>(key).ConfigureAwait(false));

                await distributedCache.RemoveAsync(key).ConfigureAwait(false);
                Assert.Null(await distributedCache.GetAsync<string>(key).ConfigureAwait(false));
            }
        }

        [Fact]
        public async Task DistributedCache_Clear_Async()
        {
            var distributedCache = GetDistributedCache();

            var keys = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var key = Guid.NewGuid().ToString("N");
                await distributedCache.SetAsync(key, "something").ConfigureAwait(false);
                keys.Add(key);
            }
            Assert.Equal("something", await distributedCache.GetAsync<string>(keys.First()).ConfigureAwait(false));
            Assert.Equal("something", await distributedCache.GetAsync<string>(keys.Last()).ConfigureAwait(false));

            await distributedCache.ClearAsync().ConfigureAwait(false);
            foreach (var key in keys)
            {
                Assert.Null(await distributedCache.GetAsync<string>(key).ConfigureAwait(false));
            }
        }

        [Fact]
        public async Task DistributedCache_Region_Async()
        {
            var distributedCache = GetDistributedCache();

            var keys = new List<string>();
            var regionKeys = new List<string>();
            var region = "test";
            var value = "something";

            for (int i = 0; i < 10; i++)
            {
                var regionKey = Guid.NewGuid().ToString("N");
                await distributedCache.SetAsync(regionKey, value, region).ConfigureAwait(false);
                regionKeys.Add(regionKey);
            }
            Assert.Equal(value, await distributedCache.GetAsync<string>(regionKeys.First(), region).ConfigureAwait(false));
            Assert.Equal(value, await distributedCache.GetAsync<string>(regionKeys.Last(), region).ConfigureAwait(false));

            await distributedCache.RemoveAsync(regionKeys.First(), region).ConfigureAwait(false);
            Assert.Null(await distributedCache.GetAsync<string>(regionKeys.First(), region).ConfigureAwait(false));

            for (int i = 0; i < 10; i++)
            {
                var key = Guid.NewGuid().ToString("N");
                await distributedCache.SetAsync(key, value).ConfigureAwait(false);
                keys.Add(key);
            }
            Assert.Equal(value, await distributedCache.GetAsync<string>(keys.First()).ConfigureAwait(false));
            Assert.Equal(value, await distributedCache.GetAsync<string>(keys.Last()).ConfigureAwait(false));
            Assert.Null(await distributedCache.GetAsync<string>(keys.First(), region).ConfigureAwait(false));

            await distributedCache.ClearRegionAsync(region).ConfigureAwait(false);
            foreach (var regionKey in regionKeys)
            {
                Assert.Null(await distributedCache.GetAsync<string>(regionKey, region).ConfigureAwait(false));
            }
            foreach (var key in keys)
            {
                Assert.Equal(value, await distributedCache.GetAsync<string>(key).ConfigureAwait(false));
            }
        }
        #endregion
    }
}
