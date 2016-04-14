using System;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCP.Cache.Redis
{
    /// <summary>
    /// Redis连接
    /// </summary>
    public class RedisConnection : IDisposable
    {
        private static IDictionary<string, ConnectionMultiplexer> connectionDic = new Dictionary<string, ConnectionMultiplexer>();
        private static object connectLock = new object();

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
        public ConnectionMultiplexer Connect()
        {
            ConnectionMultiplexer connection;
            if (!connectionDic.TryGetValue(_connectionString, out connection))
            {
                lock (connectLock)
                {
                    if (!connectionDic.TryGetValue(_connectionString, out connection))
                    {
                        connection = ConnectionMultiplexer.Connect(_configOptions);

                        CheckConnection(connection);

                        connectionDic.Add(_connectionString, connection);
                    }
                }
            }

            return connection;
        }

        public async Task<ConnectionMultiplexer> ConnectAsync()
        {
            ConnectionMultiplexer connection;
            if (!connectionDic.TryGetValue(_connectionString, out connection))
            {
                connection = await ConnectionMultiplexer.ConnectAsync(_configOptions).ConfigureAwait(false);

                CheckConnection(connection);

                if (!connectionDic.ContainsKey(_connectionString))
                {
                    lock(connectLock)
                    {
                        if (!connectionDic.ContainsKey(_connectionString))
                        {
                            connectionDic.Add(_connectionString, connection);
                        } 
                    }
                }                                    
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

        protected void CheckDisposed()
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    lock(connectLock)
                    {
                        ConnectionMultiplexer connection;
                        if (connectionDic.TryGetValue(_connectionString, out connection))
                        {
                            connectionDic.Remove(_connectionString);

                            connection.Dispose();
                        }
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
