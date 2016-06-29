using System;
using FCP.Util;

namespace FCP.Cache.Service
{
    public interface ICacheServiceBuilder
    {
        /// <summary>
        /// Build a CacheService 
        /// </summary>
        /// <returns></returns>
        ICacheService Build();

        /// <summary>
        /// Add a CacheProvider
        /// </summary>
        /// <param name="cacheProvider"></param>
        /// <returns></returns>
        ICacheServiceBuilder AddCacheProvider(Func<CacheServiceConfiguration, ICacheProvider<string>> cacheProviderFactory);

        /// <summary>
        /// Set the serializer
        /// </summary>
        /// <param name="serializer"></param>
        /// <returns></returns>
        ICacheServiceBuilder UseSerializer(ISerializer serializer);
    }
}
