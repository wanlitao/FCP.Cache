namespace FCP.Cache
{
    public interface ICacheSerializer
    {
        byte[] Serialize<TValue>(TValue value);

        TValue Deserialize<TValue>(byte[] data);
    }
}
