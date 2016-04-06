using System;
using Xunit;
using FCP.Cache.Memory;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace FCP.Cache.Test
{
    public class MemoryCacheTest
    {
        [Fact]
        public void MemoryCache_Absolute_Expire()
        {
            var key = Guid.NewGuid().ToString("N");
            var options = CacheEntryOptionsFactory.AbSolute().Timeout(TimeSpan.FromMilliseconds(300));

            var memoryCache = MemoryCacheProvider.Default;
            memoryCache.Set(key, "something", options);

            Assert.Equal("something", memoryCache.Get<string>(key));

            Thread.Sleep(300);

            Assert.Null(memoryCache.Get<string>(key));
        }

        [Fact]
        public void MemoryCache_Sliding_Expire()
        {
            var memoryCache = MemoryCacheProvider.Default;

            var key = Guid.NewGuid().ToString("N");
            var options = CacheEntryOptionsFactory.Sliding().Timeout(TimeSpan.FromMilliseconds(100));
            memoryCache.Set(key, "something", options);            

            Thread.Sleep(80);
            Assert.Equal("something", memoryCache.Get<string>(key));

            Thread.Sleep(80);
            Assert.Equal("something", memoryCache.Get<string>(key));

            Thread.Sleep(100);
            Assert.Null(memoryCache.Get<string>(key));

            options = CacheEntryOptionsFactory.Sliding().Timeout(TimeSpan.FromSeconds(2));
            memoryCache.Set(key, "something", options);

            Thread.Sleep(1800);
            Assert.Equal("something", memoryCache.Get<string>(key));

            Thread.Sleep(1800);
            Assert.Equal("something", memoryCache.Get<string>(key));

            Thread.Sleep(2000);
            Assert.Null(memoryCache.Get<string>(key));
        }

        [Fact]
        public void MemoryCache_Remove()
        {
            var key = Guid.NewGuid().ToString("N");           

            using (var memoryCache = new MemoryCacheProvider("test"))
            {
                memoryCache.Set(key, "something");
                Assert.Equal("something", memoryCache.Get<string>(key));

                memoryCache.Remove(key);
                Assert.Null(memoryCache.Get<string>(key));
            }
        }

        [Fact]
        public void MemoryCache_Clear()
        {
            var memoryCache = MemoryCacheProvider.Default;

            var keys = new List<string>();            
            for (int i = 0; i < 10; i++)
            {
                var key = Guid.NewGuid().ToString("N");
                memoryCache.Set(key, "something");
                keys.Add(key);
            }            
            Assert.Equal("something", memoryCache.Get<string>(keys.First()));
            Assert.Equal("something", memoryCache.Get<string>(keys.Last()));

            memoryCache.Clear();
            foreach(var key in keys)
            {
                Assert.Null(memoryCache.Get<string>(key));
            }
        }

        [Fact]
        public void MemoryCache_Region()
        {
            var memoryCache = MemoryCacheProvider.Default;

            var keys = new List<string>();
            var regionKeys = new List<string>();
            var region = "test";
            var value = "something";                      

            for (int i = 0; i < 10; i++)
            {
                var regionKey = Guid.NewGuid().ToString("N");
                memoryCache.Set(regionKey, value, region);
                regionKeys.Add(regionKey);
            }
            Assert.Equal(value, memoryCache.Get<string>(regionKeys.First(), region));
            Assert.Equal(value, memoryCache.Get<string>(regionKeys.Last(), region));

            memoryCache.Remove(regionKeys.First(), region);
            Assert.Null(memoryCache.Get<string>(regionKeys.First(), region));

            for (int i = 0; i < 10; i++)
            {
                var key = Guid.NewGuid().ToString("N");
                memoryCache.Set(key, value);
                keys.Add(key);
            }
            Assert.Equal(value, memoryCache.Get<string>(keys.First()));
            Assert.Equal(value, memoryCache.Get<string>(keys.Last()));
            Assert.Null(memoryCache.Get<string>(keys.First(), region));

            memoryCache.ClearRegion(region);
            foreach (var regionKey in regionKeys)
            {
                Assert.Null(memoryCache.Get<string>(regionKey, region));
            }
            foreach (var key in keys)
            {
                Assert.Equal(value, memoryCache.Get<string>(key));
            }
        }
    }
}
