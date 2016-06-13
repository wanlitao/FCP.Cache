using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FCP.Cache.Service.Test
{
    public class CacheServiceConcurrentTest
    {
        protected ICacheService GetCacheService()
        {
            return CacheServiceTestHelper.GetCacheService();
        }

        protected ICacheService GetMemoryCacheService()
        {
            return CacheServiceTestHelper.GetMemoryCacheService();
        }

        [Fact]
        public void CacheService_GetOrAdd()
        {
            var key = Guid.NewGuid().ToString("N");
            var options = CacheEntryOptionsFactory.AbSolute().Timeout(TimeSpan.FromMilliseconds(300));

            var cacheService = GetCacheService();

            cacheService.GetOrAdd(key, (k) => { return "something"; }, options);
            Assert.Equal("something", cacheService.Get<string>(key));
            Thread.Sleep(320);
            Assert.Null(cacheService.Get<string>(key));

            var region = "test";
            cacheService.GetOrAdd(key, (k) => { return "something"; }, options, region);
            Assert.Equal("something", cacheService.Get<string>(key, region));
            Thread.Sleep(320);
            Assert.Null(cacheService.Get<string>(key, region));
        }

        [Fact]
        public async Task CacheService_GetOrAdd_Async()
        {
            var key = Guid.NewGuid().ToString("N");
            var options = CacheEntryOptionsFactory.AbSolute().Timeout(TimeSpan.FromMilliseconds(300));

            var cacheService = GetCacheService();

            await cacheService.GetOrAddAsync(key,
                async (k) => { await Task.Yield(); return "something"; },
                options).ConfigureAwait(false);
            Assert.Equal("something", await cacheService.GetAsync<string>(key).ConfigureAwait(false));
            Thread.Sleep(320);
            Assert.Null(await cacheService.GetAsync<string>(key).ConfigureAwait(false));

            var region = "test";
            await cacheService.GetOrAddAsync(key,
                async (k) => { await Task.Yield(); return "something"; },
                options, region).ConfigureAwait(false);
            Assert.Equal("something", await cacheService.GetAsync<string>(key, region).ConfigureAwait(false));
            Thread.Sleep(320);
            Assert.Null(await cacheService.GetAsync<string>(key, region).ConfigureAwait(false));
        }

        [Fact]
        public void CacheService_GetOrAdd_Concurrent()
        {
            var key = Guid.NewGuid().ToString("N");
            var options = CacheEntryOptionsFactory.AbSolute().Timeout(TimeSpan.FromHours(1));

            var cacheService = GetMemoryCacheService();

            var tasks = new List<Task>();
            _value = null;

            //concurrent write
            for (int i = 0; i < 1000; i++)
            {
                Func<Task> taskFunc = async () =>
                {
                    await Task.Yield();
                    var value = await cacheService.GetOrAddAsync(key, ValueFactory, options).ConfigureAwait(false);
                    Assert.Equal(_value, value);
                };

                tasks.Add(taskFunc());
            }
            Task.WaitAll(tasks.ToArray());

            //concurrent read
            for (int i = 0; i < 1000; i++)
            {
                Func<Task> taskFunc = async () =>
                {
                    await Task.Yield();
                    var value = await cacheService.GetOrAddAsync(key, ValueFactory, options).ConfigureAwait(false);
                    Assert.Equal(_value, value);
                };

                tasks.Add(taskFunc());
            }
            Task.WaitAll(tasks.ToArray());
        }

        private volatile string _value = null;

        private async Task<string> ValueFactory(string key)
        {
            await Task.Yield();

            if (_value != null)
                Assert.True(false, "重复创建值");

            return _value = Path.GetRandomFileName();
        }
    }
}
