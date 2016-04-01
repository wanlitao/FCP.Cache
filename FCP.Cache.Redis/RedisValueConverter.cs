using System;
using StackExchange.Redis;

namespace FCP.Cache.Redis
{
    internal interface IRedisValueConverter
    {
        RedisValue ToRedisValue<TValue>(TValue value);

        TValue FromRedisValue<TValue>(RedisValue value);
    }

    internal interface IRedisValueConverter<TValue>
    {
        RedisValue ToRedisValue(TValue value);

        TValue FromRedisValue(RedisValue value);
    }

    internal class RedisValueConverter
        : IRedisValueConverter,
        IRedisValueConverter<byte[]>,
        IRedisValueConverter<string>,
        IRedisValueConverter<int>,
        IRedisValueConverter<int?>,
        IRedisValueConverter<double>,
        IRedisValueConverter<double?>,
        IRedisValueConverter<bool>,
        IRedisValueConverter<bool?>,
        IRedisValueConverter<long>,
        IRedisValueConverter<long?>
    {
        private readonly ICacheSerializer _serializer;        

        public RedisValueConverter(ICacheSerializer serializer)
        {
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            this._serializer = serializer;
        }

        #region byte[] converter
        RedisValue IRedisValueConverter<byte[]>.ToRedisValue(byte[] value)
        {
            return value;
        }

        byte[] IRedisValueConverter<byte[]>.FromRedisValue(RedisValue value)
        {
            return value;
        }
        #endregion

        #region string converter
        RedisValue IRedisValueConverter<string>.ToRedisValue(string value)
        {
            return value;
        }

        string IRedisValueConverter<string>.FromRedisValue(RedisValue value)
        {
            return value;
        }
        #endregion

        #region int converter
        RedisValue IRedisValueConverter<int>.ToRedisValue(int value)
        {
            return value;
        }

        int IRedisValueConverter<int>.FromRedisValue(RedisValue value)
        {
            return (int)value;
        }
        #endregion

        #region int? converter
        RedisValue IRedisValueConverter<int?>.ToRedisValue(int? value)
        {
            return value;
        }

        int? IRedisValueConverter<int?>.FromRedisValue(RedisValue value)
        {
            return (int?)value;
        }
        #endregion

        #region double converter
        RedisValue IRedisValueConverter<double>.ToRedisValue(double value)
        {
            return value;
        }

        double IRedisValueConverter<double>.FromRedisValue(RedisValue value)
        {
            return (double)value;
        }
        #endregion

        #region double? converter
        RedisValue IRedisValueConverter<double?>.ToRedisValue(double? value)
        {
            return value;
        }

        double? IRedisValueConverter<double?>.FromRedisValue(RedisValue value)
        {
            return (double?)value;
        }
        #endregion

        #region bool converter
        RedisValue IRedisValueConverter<bool>.ToRedisValue(bool value)
        {
            return value;
        }

        bool IRedisValueConverter<bool>.FromRedisValue(RedisValue value)
        {
            return (bool)value;
        }
        #endregion

        #region bool? converter
        RedisValue IRedisValueConverter<bool?>.ToRedisValue(bool? value)
        {
            return value;
        }

        bool? IRedisValueConverter<bool?>.FromRedisValue(RedisValue value)
        {
            return (bool?)value;
        }
        #endregion

        #region long converter
        RedisValue IRedisValueConverter<long>.ToRedisValue(long value)
        {
            return value;
        }

        long IRedisValueConverter<long>.FromRedisValue(RedisValue value)
        {
            return (long)value;
        }
        #endregion

        #region long? converter
        RedisValue IRedisValueConverter<long?>.ToRedisValue(long? value)
        {
            return value;
        }

        long? IRedisValueConverter<long?>.FromRedisValue(RedisValue value)
        {
            return (long?)value;
        }
        #endregion

        #region common converter
        RedisValue IRedisValueConverter.ToRedisValue<TValue>(TValue value)
        {
            var typedConverter = this as IRedisValueConverter<TValue>;
            if (typedConverter != null)
            {
                return typedConverter.ToRedisValue(value);
            }

            return _serializer.Serialize(value);            
        }

        TValue IRedisValueConverter.FromRedisValue<TValue>(RedisValue value)
        {
            if (value.IsNull || value.IsNullOrEmpty || !value.HasValue)            
                return default(TValue);

            var typedConverter = this as IRedisValueConverter<TValue>;
            if (typedConverter != null)
            {
                return typedConverter.FromRedisValue(value);
            }

            return _serializer.Deserialize<TValue>(value);
        }
        #endregion
    }
}
