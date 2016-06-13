using Newtonsoft.Json;

namespace FCP.Cache.Service
{
    public static class CacheServiceBuilderExtensions
    {
        public static ICacheServiceBuilder UseJsonSerializer(this ICacheServiceBuilder serviceBuilder)
        {
            return serviceBuilder.UseSerializer(new JsonCacheSerializer());
        }

        public static ICacheServiceBuilder UseJsonSerializer(this ICacheServiceBuilder serviceBuilder, JsonSerializerSettings settings)
        {
            return serviceBuilder.UseSerializer(new JsonCacheSerializer(settings));
        }
    }
}
