using FCP.Cache.Test;

namespace FCP.Cache.Service.Test
{
    public class CacheServiceTest : DistributedCacheTest
    {
        protected override IDistributedCacheProvider GetDistributedCache()
        {
            return CacheServiceTestHelper.GetCacheService("Test");
        }
    }
}
