using Jil;

namespace WebAPI.OutputCache.Redis
{
    /// <summary>
    ///     Facade for <see cref="Jil.JSON" />.
    /// </summary>
    public class JsonSerializer : IJsonSerializer
    {
        public T DeserializeObject<T>(string json)
        {
            return JSON.Deserialize<T>(json);
        }

        public string SerializeObject<T>(T value)
        {
            return JSON.Serialize(value);
        }
    }
}