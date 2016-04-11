using System;
using StackExchange.Redis;

namespace FCP.Cache.Redis
{
    public interface IRedisSentinelManager
    {
        RedisCacheProvider GetRedisCacheProvider();

        RedisCacheProvider GetRedisCacheProvider(Action<ConfigurationOptions> configurationSettings);

        RedisCacheProvider GetRedisCacheProvider(ICacheSerializer cacheSerializer);

        RedisCacheProvider GetRedisCacheProvider(Action<ConfigurationOptions> configurationSettings, ICacheSerializer cacheSerializer);
    }
}
