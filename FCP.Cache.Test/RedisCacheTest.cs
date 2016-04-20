using FCP.Cache.Redis;

namespace FCP.Cache.Test
{
    public class RedisCacheTest : DistributedCacheTest
    {
        protected const string redisConfiguration = "localhost,allowAdmin=true";        

        protected override IDistributedCacheProvider GetDistributedCache()
        {
            return new RedisCacheProvider(redisConfiguration);
        }        
    }
}
