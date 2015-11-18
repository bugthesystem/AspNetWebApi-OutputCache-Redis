namespace WebAPI.OutputCache.Redis
{
    public interface IJsonSerializer
    {
        T DeserializeObject<T>(string json);
        string SerializeObject<T>(T value);
    }
}