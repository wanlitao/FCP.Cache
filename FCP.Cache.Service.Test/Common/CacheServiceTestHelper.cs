using FCP.Cache.Service.Memory;
using FCP.Cache.Service.Redis;

namespace FCP.Cache.Service.Test
{
    internal class CacheServiceTestHelper
    {
        internal const string RedisConfiguration = "localhost,allowAdmin=true";

        internal const string RedisMasterName = "redismaster1";

        internal static string[] RedisSentinelHosts = new[]
        {
            "127.0.0.1",
            "127.0.0.1:32001",
            "127.0.0.1:32002"
        };

        internal static ICacheService GetMemoryCacheService()
        {
            return new CacheServiceBuilder()
                .AddMemoryCache()                
                .Build();
        }

        internal static ICacheService GetCacheService()
        {
            return new CacheServiceBuilder()
                .AddMemoryCache()
                .AddRedisCache(RedisConfiguration)
                .UseJsonSerializer()                
                .Build();
        }

        internal static ICacheService GetCacheService(string memoryName)
        {
            return new CacheServiceBuilder()
                .AddMemoryCache(memoryName)
                .AddRedisCache(RedisConfiguration)
                .UseJsonSerializer()
                .Build();
        }

        internal static ICacheService GetCacheServiceBySentinel()
        {
            return new CacheServiceBuilder()
                .AddMemoryCache()
                .AddRedisCacheBySentinel(RedisMasterName, RedisSentinelHosts)
                .UseJsonSerializer()
                .Build();
        }
    }
}
