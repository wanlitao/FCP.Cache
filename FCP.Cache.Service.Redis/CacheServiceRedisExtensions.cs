using FCP.Cache.Redis;
using StackExchange.Redis;
using System;

namespace FCP.Cache.Service.Redis
{
    public static class CacheServiceRedisExtensions
    {
        public static ICacheServiceBuilder UseRedisCache(this ICacheServiceBuilder serviceBuilder, string configuration)
        {
            var configOptions = RedisCacheProvider.ParseConfigurationOptions(configuration);

            return serviceBuilder.UseRedisCache(configOptions);
        }

        public static ICacheServiceBuilder UseRedisCache(this ICacheServiceBuilder serviceBuilder, ConfigurationOptions configOptions)
        {
            return serviceBuilder.UseCacheProvider((configuration) =>
            {
                return new RedisCacheProvider(configOptions, configuration.Serializer);
            });
        }

        #region Sentinel
        public static ICacheServiceBuilder UseRedisCacheWithSentinel(this ICacheServiceBuilder serviceBuilder,
            Action<ConfigurationOptions> configurationSettings = null)
        {
            var sentinelManager = new RedisSentinelManager();

            return serviceBuilder.UseRedisCacheWithSentinel(sentinelManager, configurationSettings);
        }

        public static ICacheServiceBuilder UseRedisCacheWithSentinel(this ICacheServiceBuilder serviceBuilder, string masterName,
            Action<ConfigurationOptions> configurationSettings = null)
        {
            var sentinelManager = new RedisSentinelManager(masterName);

            return serviceBuilder.UseRedisCacheWithSentinel(sentinelManager, configurationSettings);
        }

        public static ICacheServiceBuilder UseRedisCacheWithSentinel(this ICacheServiceBuilder serviceBuilder, string masterName,
            Action<ConfigurationOptions> configurationSettings = null, params string[] sentinelHosts)
        {
            var sentinelManager = new RedisSentinelManager(masterName, sentinelHosts);

            return serviceBuilder.UseRedisCacheWithSentinel(sentinelManager, configurationSettings);
        }

        private static ICacheServiceBuilder UseRedisCacheWithSentinel(this ICacheServiceBuilder serviceBuilder, IRedisSentinelManager sentinelManager,
            Action<ConfigurationOptions> configurationSettings)
        {
            return serviceBuilder.UseCacheProvider((configuration) =>
            {
                return sentinelManager.GetRedisCacheProvider(configurationSettings, configuration.Serializer);
            });
        }
        #endregion
    }
}
