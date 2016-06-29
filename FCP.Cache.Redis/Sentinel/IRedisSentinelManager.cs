using System;
using StackExchange.Redis;
using FCP.Util;

namespace FCP.Cache.Redis
{
    public interface IRedisSentinelManager : IDisposable
    {
        RedisCacheProvider GetRedisCacheProvider();

        RedisCacheProvider GetRedisCacheProvider(Action<ConfigurationOptions> configurationSettings);

        RedisCacheProvider GetRedisCacheProvider(ISerializer cacheSerializer);

        RedisCacheProvider GetRedisCacheProvider(Action<ConfigurationOptions> configurationSettings, ISerializer cacheSerializer);
    }
}
