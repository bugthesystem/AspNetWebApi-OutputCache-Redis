namespace WebAPI.OutputCache.Redis
{
    public interface IRedisConnectionSettings
    {
        int Db { get; set; }
        string ConnectionString { get; set; }
    }
}