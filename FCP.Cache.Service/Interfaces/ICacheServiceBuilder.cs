using System;

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
        ICacheServiceBuilder UseCacheProvider(Func<CacheServiceConfiguration, ICacheProvider<string>> cacheProviderFactory);

        /// <summary>
        /// Set the serializer
        /// </summary>
        /// <param name="serializer"></param>
        /// <returns></returns>
        ICacheServiceBuilder UseSerializer(ICacheSerializer serializer);
    }
}
