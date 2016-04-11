using System;
using StackExchange.Redis;

namespace FCP.Cache.Redis
{
    public class RedisSentinelException : Exception
    {
        internal RedisSentinelException(string message)
            : base(message)
        { }

        public RedisSentinelException(string message, RedisException innerException)
            : base(message, innerException)
        { }
    }
}
