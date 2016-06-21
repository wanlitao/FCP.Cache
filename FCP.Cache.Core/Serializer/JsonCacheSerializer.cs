using Newtonsoft.Json;
using System;
using System.Text;

namespace FCP.Cache
{
    /// <summary>
    /// Newtonsoft.Json Serializer
    /// </summary>
    public class JsonCacheSerializer : ICacheSerializer
    {
        public JsonCacheSerializer()
        {
            SerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        }

        public JsonCacheSerializer(JsonSerializerSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings)); 

            SerializerSettings = settings;
        }

        public JsonSerializerSettings SerializerSettings { get; private set; }

        public byte[] Serialize<TValue>(TValue value)
        {
            if (value == null)
                return null;

            var stringValue = JsonConvert.SerializeObject(value, SerializerSettings);
            return Encoding.UTF8.GetBytes(stringValue);            
        }

        public TValue Deserialize<TValue>(byte[] data)
        {
            if (data == null || data.Length < 1)
                return default(TValue);

            var stringValue = Encoding.UTF8.GetString(data, 0, data.Length);
            return JsonConvert.DeserializeObject<TValue>(stringValue, SerializerSettings);
        }
    }
}
