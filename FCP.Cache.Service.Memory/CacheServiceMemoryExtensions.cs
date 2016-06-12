using FCP.Cache.Memory;

namespace FCP.Cache.Service.Memory
{
    public static class CacheServiceMemoryExtensions
    {
        public static ICacheServiceBuilder UseMemoryCache(this ICacheServiceBuilder serviceBuilder, string name = null)
        {
            return serviceBuilder.UseCacheProvider((configuration) =>
            {
                if (string.IsNullOrEmpty(name))
                    return MemoryCacheProvider.Default;

                return new MemoryCacheProvider(name);
            });
        }
    }
}
