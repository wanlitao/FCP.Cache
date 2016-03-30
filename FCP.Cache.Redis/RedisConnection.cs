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
    public class RedisConnection
    {
        private static IDictionary<string, ConnectionMultiplexer> connectionDic = new Dictionary<string, ConnectionMultiplexer>();
        private static object connectLock = new object();

        private readonly string _connectionString;

        public RedisConnection(string configuration)
        {
            if (string.IsNullOrEmpty(configuration))
                throw new ArgumentNullException(nameof(configuration));

            _connectionString = GetConnectionString(configuration);
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static string GetConnectionString(string configuration)
        {
            var config = ConfigurationOptions.Parse(configuration);

            if (config.EndPoints.Count == 0)
                throw new ArgumentException("No endpoints specified", "configuration");

            config.SetDefaultPorts();

            return config.ToString();
        }


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
                        connection = ConnectionMultiplexer.Connect(_connectionString);

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
                connection = await ConnectionMultiplexer.ConnectAsync(_connectionString).ConfigureAwait(false);

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
    }
}
