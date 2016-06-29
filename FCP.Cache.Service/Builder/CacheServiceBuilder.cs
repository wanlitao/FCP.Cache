using System;
using System.Collections.Generic;
using FCP.Util;

namespace FCP.Cache.Service
{
    public class CacheServiceBuilder : ICacheServiceBuilder
    {
        private readonly CacheServiceConfiguration _configuration;
        private readonly List<Func<CacheServiceConfiguration, ICacheProvider<string>>> _cacheProviderFactories;

        public CacheServiceBuilder()
        {
            _configuration = new CacheServiceConfiguration();
            _cacheProviderFactories = new List<Func<CacheServiceConfiguration, ICacheProvider<string>>>();
        }

        public ICacheServiceBuilder AddCacheProvider(Func<CacheServiceConfiguration, ICacheProvider<string>> cacheProviderFactory)
        {
            if (cacheProviderFactory == null)
                throw new ArgumentNullException(nameof(cacheProviderFactory));

            _cacheProviderFactories.Add(cacheProviderFactory);

            return this;
        }

        public ICacheServiceBuilder UseSerializer(ISerializer serializer)
        {
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            _configuration.Serializer = serializer;

            return this;
        }

        public ICacheService Build()
        {
            var cacheProviders = BuildCacheProviders();

            return new DefaultCacheService(cacheProviders);
        }

        private ICollection<IDistributedCacheProvider> BuildCacheProviders()
        {
            var cacheProviders = new List<IDistributedCacheProvider>();

            foreach(var cacheProviderFactory in _cacheProviderFactories)
            {
                var cacheProvider = cacheProviderFactory(_configuration);

                cacheProviders.Add((cacheProvider as IDistributedCacheProvider) ?? cacheProvider.AsDistributedCache());
            }

            return cacheProviders;
        }
    }
}
