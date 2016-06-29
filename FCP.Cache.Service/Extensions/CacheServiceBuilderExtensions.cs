using Newtonsoft.Json;
using FCP.Util;

namespace FCP.Cache.Service
{
    public static class CacheServiceBuilderExtensions
    {
        public static ICacheServiceBuilder UseJsonSerializer(this ICacheServiceBuilder serviceBuilder)
        {
            return serviceBuilder.UseSerializer(SerializerFactory.JsonSerializer);
        }

        public static ICacheServiceBuilder UseJsonSerializer(this ICacheServiceBuilder serviceBuilder, JsonSerializerSettings settings)
        {
            return serviceBuilder.UseSerializer(SerializerFactory.GetJsonSerializer(settings));
        }
    }
}
