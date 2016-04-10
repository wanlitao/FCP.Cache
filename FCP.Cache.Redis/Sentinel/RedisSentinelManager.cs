using System;
using StackExchange.Redis;

namespace FCP.Cache.Redis
{
    /// <summary>
    /// Redis Sentinel Support
    /// </summary>
    public class RedisSentinelManager : IRedisSentinelManager
    {
        private const string defaultMasterName = "mymaster";
        private const int defaultSentinelPort = 26379;        
        private static string defaultSentinelHost = string.Format("{0}:{1}", "127.0.0.1", defaultSentinelPort);

        private const int sentinelSyncTimeout = 5000;

        private readonly string _masterName;
        private readonly EndPointCollection _sentinelEndpoints;
        private RedisConnection _sentinelConnection;
        private int _sentinelIndex = 0;

        public RedisSentinelManager()
            : this(defaultMasterName, new string[] { defaultSentinelHost })
        { }

        public RedisSentinelManager(string masterName, params string[] sentinelHosts)
        {
            if (string.IsNullOrEmpty(masterName))
                throw new ArgumentNullException(nameof(masterName));

            if (sentinelHosts == null || sentinelHosts.Length == 0)
                throw new ArgumentException("sentinels must have at least one entry");

            _masterName = masterName;
            _sentinelEndpoints = ParseSentinelEndPoints(sentinelHosts);

            _sentinelConnection = BuildSentinelConnection(_masterName, _sentinelEndpoints);            
        }

        #region Helper functions
        protected static RedisConnection BuildSentinelConnection(string masterName, EndPointCollection sentinelEndpoints)
        {
            if (string.IsNullOrEmpty(masterName))
                return null;

            if (sentinelEndpoints == null || sentinelEndpoints.Count == 0)
                return null;

            var configOptions = new ConfigurationOptions()
            {
                CommandMap = CommandMap.Sentinel,
                AllowAdmin = true,
                TieBreaker = "",
                ServiceName = masterName,
                SyncTimeout = sentinelSyncTimeout
            };
            foreach (var sentinelEndPoint in sentinelEndpoints)
            {
                configOptions.EndPoints.Add(sentinelEndPoint);
            }

            return new RedisConnection(configOptions);
        }

        protected static EndPointCollection ParseSentinelEndPoints(params string[] sentinelHosts)
        {
            if (sentinelHosts == null || sentinelHosts.Length == 0)
                return null;

            var sentinelEndPoints = new EndPointCollection();
            for (var i = 0; i < sentinelHosts.Length; i++)
            {
                var sentinelHost = sentinelHosts[i];
                var hostPortArr = sentinelHost.Split(':');
                if (hostPortArr.Length > 2)
                    continue;  //invalid hostAndPort string

                if (hostPortArr.Length == 1)
                    sentinelHost = string.Format("{0}:{1}", hostPortArr[0], defaultSentinelPort);

                sentinelEndPoints.Add(sentinelHost);
            }

            return sentinelEndPoints;
        }
        #endregion
    }
}
