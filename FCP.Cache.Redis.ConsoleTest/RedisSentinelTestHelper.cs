using System.IO;

namespace FCP.Cache.Redis.ConsoleTest
{
    internal class RedisSentinelTestHelper
    {
        internal const string MasterName = "redismaster1";

        internal static string[] SentinelHosts = new[]
        {
            "127.0.0.1",
            "127.0.0.1:32001",
            "127.0.0.1:32002"
        };

        internal static IRedisSentinelManager GetSentinelManager(TextWriter logger = null)
        {
            return new RedisSentinelManager(MasterName, SentinelHosts) { SentinelLogger = logger };
        }
    }
}
