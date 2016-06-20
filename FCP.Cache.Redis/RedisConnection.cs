using System;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace FCP.Cache.Redis
{
    /// <summary>
    /// Redis连接
    /// </summary>
    public class RedisConnection : IDisposable
    {
        private static ConcurrentDictionary<string, ConnectionMultiplexer> connectionDict = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        private static ConcurrentTaskManager _taskManager = new ConcurrentTaskManager();        

        private readonly string _connectionString;
        private readonly ConfigurationOptions _configOptions;

        #region Constructor       

        public RedisConnection(ConfigurationOptions configOptions)
        {
            CheckConfigurationOptions(configOptions);

            _configOptions = configOptions;
            _connectionString = _configOptions.ToString();
        }

        /// <summary>
        /// 校验连接配置
        /// </summary>
        /// <param name="configOptions"></param>
        private static void CheckConfigurationOptions(ConfigurationOptions configOptions)
        {
            if (configOptions == null)
                throw new ArgumentNullException(nameof(configOptions));

            if (configOptions.EndPoints.Count == 0)
                throw new ArgumentException("No endpoints specified", "configuration");

            configOptions.SetDefaultPorts();
        }
        #endregion

        #region Connect
        public ConnectionMultiplexer Connect(TextWriter connectLogger = null)
        {
            return connectionDict.GetOrAdd(_connectionString, (connectionStr) =>
            {
                return _taskManager.GetTaskResult(connectionStr, () =>
                {
                    //again check to avoid the ConnectionMultiplexer.Connect execute more than once in concurrent state
                    var connection = connectionDict.GetOrAdd(connectionStr, (connectStr) =>
                    {
                        var multiplexer = ConnectionMultiplexer.Connect(_configOptions, connectLogger);

                        CheckConnection(multiplexer);

                        return multiplexer;
                    });

                    return Task.FromResult(connection);                    
                });
            });
        }

        public async Task<ConnectionMultiplexer> ConnectAsync(TextWriter connectLogger = null)
        {
            ConnectionMultiplexer connection;
            if (!connectionDict.TryGetValue(_connectionString, out connection))
            {
                connection = await _taskManager.GetTaskResultAsync(_connectionString, async () =>
                {
                    ConnectionMultiplexer multiplexer;
                    //again check to avoid the ConnectionMultiplexer.Connect execute more than once in concurrent state
                    if (!connectionDict.TryGetValue(_connectionString, out multiplexer))
                    {
                        multiplexer = await ConnectionMultiplexer.ConnectAsync(_configOptions, connectLogger).ConfigureAwait(false);

                        CheckConnection(multiplexer);

                        multiplexer = connectionDict.GetOrAdd(_connectionString, multiplexer);
                    }

                    return multiplexer;

                }).ConfigureAwait(false);
            }

            return connection;
        }

        /// <summary>
        /// 校验Redis连接
        /// </summary>
        /// <param name="connection"></param>
        private void CheckConnection(IConnectionMultiplexer connection)
        {
            if (connection == null)
            {
                throw new InvalidOperationException(
                    string.Format("Couldn't establish a connection for '{0}'.", _connectionString));
            }

            if (!connection.IsConnected)
            {
                connection.Dispose();
                throw new InvalidOperationException("Connection failed.");
            }

            var endpoints = connection.GetEndPoints();
            if (!endpoints.Select(p => connection.GetServer(p))
                .Any(p => !p.IsSlave || p.AllowSlaveWrites))
            {
                throw new InvalidOperationException("No writeable endpoint found.");
            }
        }
        #endregion

        #region Configuration
        internal ConfigurationOptions Configuration
        {
            get { return _configOptions; }
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
                    ConnectionMultiplexer connection;
                    if (connectionDict.TryRemove(_connectionString, out connection))
                    {
                        connection.Dispose();
                    }
                }
                disposedValue = true;
            }
        }

        ~RedisConnection()
        {
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
