using Jil;

namespace WebAPI.OutputCache.Redis
{
    public interface IJsonSerializer
    {
        T DeserializeObject<T>(string json, Options options= null);
        string SerializeObject<T>(T value, Options options=null);
    }
}