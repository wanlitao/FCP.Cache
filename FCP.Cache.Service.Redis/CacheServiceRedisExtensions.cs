using FCP.Cache.Redis;
using StackExchange.Redis;
using System;

namespace FCP.Cache.Service.Redis
{
    public static class CacheServiceRedisExtensions
    {
        public static ICacheServiceBuilder AddRedisCache(this ICacheServiceBuilder serviceBuilder, string configuration)
        {
            var configOptions = RedisCacheProvider.ParseConfigurationOptions(configuration);

            return serviceBuilder.AddRedisCache(configOptions);
        }

        public static ICacheServiceBuilder AddRedisCache(this ICacheServiceBuilder serviceBuilder, ConfigurationOptions configOptions)
        {
            return serviceBuilder.AddCacheProvider((configuration) =>
            {
                return new RedisCacheProvider(configOptions, configuration.Serializer);
            });
        }

        #region Sentinel
        public static ICacheServiceBuilder AddRedisCacheBySentinel(this ICacheServiceBuilder serviceBuilder,
            Action<ConfigurationOptions> configurationSettings = null)
        {
            var sentinelManager = new RedisSentinelManager();

            return serviceBuilder.AddRedisCacheBySentinel(sentinelManager, configurationSettings);
        }

        public static ICacheServiceBuilder AddRedisCacheBySentinel(this ICacheServiceBuilder serviceBuilder, string masterName,
            Action<ConfigurationOptions> configurationSettings = null)
        {
            var sentinelManager = new RedisSentinelManager(masterName);

            return serviceBuilder.AddRedisCacheBySentinel(sentinelManager, configurationSettings);
        }

        public static ICacheServiceBuilder AddRedisCacheBySentinel(this ICacheServiceBuilder serviceBuilder, string masterName, params string[] sentinelHosts)
        {
            return serviceBuilder.AddRedisCacheBySentinel(masterName, null, sentinelHosts);
        }

        public static ICacheServiceBuilder AddRedisCacheBySentinel(this ICacheServiceBuilder serviceBuilder, string masterName,
            Action<ConfigurationOptions> configurationSettings, params string[] sentinelHosts)
        {
            var sentinelManager = new RedisSentinelManager(masterName, sentinelHosts);

            return serviceBuilder.AddRedisCacheBySentinel(sentinelManager, configurationSettings);
        }

        private static ICacheServiceBuilder AddRedisCacheBySentinel(this ICacheServiceBuilder serviceBuilder, IRedisSentinelManager sentinelManager,
            Action<ConfigurationOptions> configurationSettings)
        {
            return serviceBuilder.AddCacheProvider((configuration) =>
            {
                return sentinelManager.GetRedisCacheProvider(configurationSettings, configuration.Serializer);
            });
        }
        #endregion
    }
}
