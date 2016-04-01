using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FCP.Cache
{
    /// <summary>
    /// Basic binary serialization
    /// </summary>
    public class BinaryCacheSerializer : ICacheSerializer
    {
        public byte[] Serialize<TValue>(TValue value)
        {
            if (value == null)
                return null;            

            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, value);                 
                return memoryStream.ToArray();
            }
        }

        public TValue Deserialize<TValue>(byte[] data)
        {
            if (data == null || data.Length < 1)
                return default(TValue);            

            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream(data))
            {
                var result = binaryFormatter.Deserialize(memoryStream);
                return (TValue)result;
            }
        }
    }
}
