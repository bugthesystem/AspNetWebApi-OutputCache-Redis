namespace WebAPI.OutputCache.Redis
{
    public class RedisConnectionSettings : IRedisConnectionSettings
    {
        public int Db { get; set; }
        public string ConnectionString { get; set; }
    }
}