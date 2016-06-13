using FCP.Cache.Memory;

namespace FCP.Cache.Service.Memory
{
    public static class CacheServiceMemoryExtensions
    {
        public static ICacheServiceBuilder AddMemoryCache(this ICacheServiceBuilder serviceBuilder)
        {
            return serviceBuilder.AddCacheProvider((configuration) =>
            {
                return MemoryCacheProvider.Default;
            });
        }

        public static ICacheServiceBuilder AddMemoryCache(this ICacheServiceBuilder serviceBuilder, string name)
        {
            return serviceBuilder.AddCacheProvider((configuration) =>
            {
                return new MemoryCacheProvider(name);
            });
        }
    }
}
