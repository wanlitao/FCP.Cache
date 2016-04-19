using System;
using System.Threading;
using System.Threading.Tasks;

namespace FCP.Cache.Redis.ConsoleTest
{
    class Program
    {
        protected static IRedisSentinelManager sentinelManager = RedisSentinelTestHelper.GetSentinelManager(Console.Out);

        static void Main(string[] args)
        {
            ExecuteRedisCacheAction(RedisCache_Absolute_Expire);

            Console.Write(Environment.NewLine);
            ExecuteRedisCacheActionAsync(RedisCache_Absolute_Expire_Async);

            Console.ReadLine();
        }

        protected static void ExecuteRedisCacheAction(Action<RedisCacheProvider> redisCacheAction)
        {
            using (var redisCache = sentinelManager.GetRedisCacheProvider())
            {                        
                redisCacheAction(redisCache);               

                Console.Write(Environment.NewLine);
                Console.WriteLine("Please change the redis master slave state.........");
                Console.ReadLine();                
                
                redisCacheAction(redisCache);                
            }
        }

        protected static void ExecuteRedisCacheActionAsync(Func<RedisCacheProvider, Task> redisCacheAsyncAction)
        {
            using (var redisCache = sentinelManager.GetRedisCacheProvider())
            {
                redisCacheAsyncAction(redisCache).Wait();                

                Console.Write(Environment.NewLine);
                Console.WriteLine("Please change the redis master slave state.........");
                Console.ReadLine();                
                
                redisCacheAsyncAction(redisCache).Wait();                
            }
        }


        #region sync test
        protected static void RedisCache_Absolute_Expire(RedisCacheProvider redisCache)
        {
            Console.WriteLine("Sync test absolute expire start...");
            Console.Write(Environment.NewLine);            

            var key = Guid.NewGuid().ToString("N");
            var value = "something";
            var options = CacheEntryOptionsFactory.AbSolute().Timeout(TimeSpan.FromMilliseconds(300));

            redisCache.Set(key, value, options);
            Console.WriteLine(string.Format("Set key:{0} with value:{1} by expire:{2}",
                key, value, options.ExpirationTimeout.TotalMilliseconds));

            Console.WriteLine(string.Format("Get key:{0} with value:{1}", key, redisCache.Get<string>(key)));

            Thread.Sleep(300);
            Console.WriteLine(string.Format("After expire:{0} get key:{1} with value:{2}",
                options.ExpirationTimeout.TotalMilliseconds, key, redisCache.Get<string>(key)));

            Console.Write(Environment.NewLine);
            Console.WriteLine("Sync test absolute expire end...");            
        }
        #endregion

        #region async test
        protected static async Task RedisCache_Absolute_Expire_Async(RedisCacheProvider redisCache)
        {
            Console.WriteLine("Async test absolute expire start...");
            Console.Write(Environment.NewLine);

            var key = Guid.NewGuid().ToString("N");
            var value = "something";
            var options = CacheEntryOptionsFactory.AbSolute().Timeout(TimeSpan.FromMilliseconds(300));

            await redisCache.SetAsync(key, value, options).ConfigureAwait(false);
            Console.WriteLine(string.Format("Async set key:{0} with value:{1} by expire:{2}",
                key, value, options.ExpirationTimeout.TotalMilliseconds));

            Console.WriteLine(string.Format("Async get key:{0} with value:{1}", key, await redisCache.GetAsync<string>(key).ConfigureAwait(false)));

            Thread.Sleep(350);
            Console.WriteLine(string.Format("After expire:{0} async get key:{1} with value:{2}",
                options.ExpirationTimeout.TotalMilliseconds, key, await redisCache.GetAsync<string>(key).ConfigureAwait(false)));

            Console.Write(Environment.NewLine);
            Console.WriteLine("Async test absolute expire end...");
        }
        #endregion
    }
}
