using System;
using System.Threading;

namespace FCP.Cache.Redis.ConsoleTest
{
    class Program
    {
        protected static IRedisSentinelManager sentinelManager = RedisSentinelTestHelper.GetSentinelManager(Console.Out);

        static void Main(string[] args)
        {
            ExecuteRedisCacheAction(RedisCache_Absolute_Expire);

            Console.ReadLine();
        }

        protected static void ExecuteRedisCacheAction(Action<RedisCacheProvider> redisCacheAction)
        {
            using (var redisCache = sentinelManager.GetRedisCacheProvider())
            {
                Console.WriteLine("...............................");
                redisCacheAction(redisCache);
                Console.WriteLine("...............................");
            }
        }

        protected static void RedisCache_Absolute_Expire(RedisCacheProvider redisCache)
        {
            Console.WriteLine("Test absolute expire start...");
            Console.Write(Environment.NewLine);            

            var key = Guid.NewGuid().ToString("N");
            var value = "something";
            var options = CacheEntryOptionsFactory.AbSolute().Timeout(TimeSpan.FromMilliseconds(300));

            redisCache.Set(key, value, options);
            Console.WriteLine(string.Format("Set key:{0} with value:{1} by expire:{2}",
                key, value, options.ExpirationTimeout.TotalMilliseconds));

            Console.WriteLine(string.Format("Get key:{0} with value:{1}", key, redisCache.Get<string>(key)));

            Thread.Sleep(300);
            Console.WriteLine(string.Format("After expire:{0} Get key:{1} with value:{2}",
                options.ExpirationTimeout.TotalMilliseconds, key, redisCache.Get<string>(key)));

            Console.Write(Environment.NewLine);
            Console.WriteLine("Test absolute expire end...");            
        }
    }
}
