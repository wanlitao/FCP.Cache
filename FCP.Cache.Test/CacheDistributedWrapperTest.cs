using FCP.Cache.Memory;

namespace FCP.Cache.Test
{
    public class CacheDistributedWrapperTest : DistributedCacheTest
    {
        protected override IDistributedCacheProvider GetDistributedCache()
        {
            return new MemoryCacheProvider("test").AsDistributedCache();
        }
    }
}
