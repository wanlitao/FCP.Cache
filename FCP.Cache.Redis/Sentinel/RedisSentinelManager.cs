using System;
using StackExchange.Redis;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.IO;

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

        private const int sentinelConnectTimeout = 200;
        private const int sentinelSyncTimeout = 500;

        private readonly string _masterName;
        private readonly EndPointCollection _sentinelEndpoints;
        private readonly RedisConnection _sentinelConnection;

        private int _sentinelIndex = -1;
        private IServer _currentSentinelServer;

        #region properties
        public TextWriter SentinelLogger { get; set; }

        public Action<string, string> OnSentinelMessageReceived { get; set; }
        #endregion

        public RedisSentinelManager()
            : this(defaultMasterName)
        { }

        public RedisSentinelManager(string masterName)
            : this(masterName, new string[] { defaultSentinelHost })
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

            BeginListeningForConfigurationChanges();            
        }

        #region Sentinel Connection
        protected static RedisConnection BuildSentinelConnection(string masterName, EndPointCollection sentinelEndpoints)
        {
            if (string.IsNullOrEmpty(masterName))
                return null;

            var configOptions = sentinelEndpoints.ConvertConfigurationOptions();
            if (configOptions == null)
                return null;

            configOptions.CommandMap = CommandMap.Sentinel;
            configOptions.AllowAdmin = true;
            configOptions.TieBreaker = "";
            configOptions.ServiceName = masterName;
            configOptions.ConnectTimeout = sentinelConnectTimeout;
            configOptions.SyncTimeout = sentinelSyncTimeout;            

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

        #region Sentinel Server
        protected IServer GetNextSentinelServer()
        {
            if (++_sentinelIndex >= _sentinelEndpoints.Count)
                _sentinelIndex = 0;

            var nextSentinelEndPoint = _sentinelEndpoints[_sentinelIndex];

            return _sentinelConnection.Connect(SentinelLogger).GetServer(nextSentinelEndPoint);
        }

        protected IServer GetValidSentinelServer()
        {
            int connectFailCount = 0;
            int masterAddressFailCount = 0;

            RedisException lastEx = null;

            while (connectFailCount + masterAddressFailCount < _sentinelEndpoints.Count)
            {
                try
                {
                    _currentSentinelServer = _currentSentinelServer ?? GetNextSentinelServer();
                    _currentSentinelServer.Ping();

                    var masterEndPoint = _currentSentinelServer.SentinelGetMasterAddressByName(_masterName);
                    if (masterEndPoint != null)
                    {
                        return _currentSentinelServer;
                    }
                    masterAddressFailCount++;  //获取master地址失败
                }
                catch (RedisException ex)
                {
                    lastEx = ex;
                    connectFailCount++;  //连接失败

                    _currentSentinelServer = null;
                }
            }

            throw FormatSentinelGetMasterException(connectFailCount, masterAddressFailCount, lastEx);
        }

        protected RedisSentinelException FormatSentinelGetMasterException(int connectFailCount,
            int masterAddressFailCount, RedisException innerException)
        {
            var errorMessage = string.Empty;

            if (masterAddressFailCount == 0)
                errorMessage = "No Redis Sentinels were available";
            else if (connectFailCount == 0)
                errorMessage = string.Format("Sentinels don't know the master name: {0}", _masterName);
            else
                errorMessage = "Can't find the master address";

            return new RedisSentinelException(errorMessage, innerException);
        }
        #endregion

        #region Sentinel Subscriber
        protected void BeginListeningForConfigurationChanges()
        {
            var sub = _sentinelConnection.Connect(SentinelLogger).GetSubscriber();
            sub.Subscribe("*", SentinelMessageReceived);
        }

        protected void SentinelMessageReceived(RedisChannel channel, RedisValue message)
        {
            if (SentinelLogger != null)
                SentinelLogger.WriteLine(string.Format("Received '{0}' on channel '{1}' from Sentinel", message, channel));

            var channelName = ((string)channel).ToLower();

            if (channelName == "+failover-end" || channelName == "+switch-master")
            {

            }
            else if(channelName == "+sentinel")
            {

            }

            if (OnSentinelMessageReceived != null)
                OnSentinelMessageReceived(channel, message);
        }
        #endregion

        #region MasterSlave Connection
        protected static EndPointCollection ParseSlaveEndPoints(params KeyValuePair<string, string>[][] slaveInfos)
        {
            if (slaveInfos == null || slaveInfos.Length == 0)
                return null;

            var slaveEndPoints = new EndPointCollection();

            string ip, port, flags;
            foreach (var slaveInfo in slaveInfos.Select(m => m.ToDictionary()))
            {
                slaveInfo.TryGetValue("flags", out flags);
                slaveInfo.TryGetValue("ip", out ip);
                slaveInfo.TryGetValue("port", out port);

                if (!string.IsNullOrEmpty(ip) && !string.IsNullOrEmpty(port)
                    && !flags.Contains("s_down") && !flags.Contains("o_down"))
                {
                    slaveEndPoints.Add(string.Format("{0}:{1}", ip, port));
                }
            }

            return slaveEndPoints;
        }

        protected static ConfigurationOptions BuildMasterSlaveConfigurationOptions(
            EndPoint masterEndPoint, EndPointCollection slaveEndPoints, Action<ConfigurationOptions> configurationSettings)
        {
            if (masterEndPoint == null)
                return null;

            var fullEndPoints = slaveEndPoints ?? new EndPointCollection();
            fullEndPoints.Add(masterEndPoint);

            var configOptions = fullEndPoints.ConvertConfigurationOptions();

            if (configurationSettings != null)
                configurationSettings(configOptions);

            return configOptions;
        }
        #endregion

        #region MasterSlave Cache Provider
        public RedisCacheProvider GetRedisCacheProvider()
        {
            return GetRedisCacheProvider(null, null);
        }

        public RedisCacheProvider GetRedisCacheProvider(Action<ConfigurationOptions> configurationSettings)
        {
            return GetRedisCacheProvider(configurationSettings, null);
        }

        public RedisCacheProvider GetRedisCacheProvider(ICacheSerializer cacheSerializer)
        {
            return GetRedisCacheProvider(null, cacheSerializer);
        }

        public RedisCacheProvider GetRedisCacheProvider(Action<ConfigurationOptions> configurationSettings, ICacheSerializer cacheSerializer)
        {
            var sentinelServer = GetValidSentinelServer();

            var masterEndPoint = sentinelServer.SentinelGetMasterAddressByName(_masterName);
            var slaveEndPoints = ParseSlaveEndPoints(sentinelServer.SentinelSlaves(_masterName));

            var configOptions = BuildMasterSlaveConfigurationOptions(masterEndPoint, slaveEndPoints, configurationSettings);

            if (cacheSerializer != null)
                return new RedisCacheProvider(configOptions, cacheSerializer);

            return new RedisCacheProvider(configOptions);
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _currentSentinelServer = null;
                    _sentinelConnection.Dispose();
                }
                disposedValue = true;
            }
        }
        
        ~RedisSentinelManager() {           
            Dispose(false);
        }
        
        public void Dispose()
        {            
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}