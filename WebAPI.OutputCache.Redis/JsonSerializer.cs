using Jil;

namespace WebAPI.OutputCache.Redis
{
    /// <summary>
    ///     Facade for <see cref="Jil.JSON" />.
    /// </summary>
    public class JsonSerializer : IJsonSerializer
    {
        public T DeserializeObject<T>(string json, Options options = null)
        {
            return JSON.Deserialize<T>(json,options);
        }

        public string SerializeObject<T>(T value, Options options = null)
        {
            return JSON.Serialize(value, options);
        }
    }
}