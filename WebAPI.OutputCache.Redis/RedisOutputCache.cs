using System;
using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;
using WebApi.OutputCache.Core.Cache;

namespace WebAPI.OutputCache.Redis
{
    public class RedisOutputCache : IApiOutputCache
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IRedisConnectionSettings _connectionSettings;
        private readonly IConnectionMultiplexer _multiplexer;

        public RedisOutputCache(IJsonSerializer jsonSerializer, IRedisConnectionSettings connectionSettings)
        {
            _jsonSerializer = jsonSerializer;
            _connectionSettings = connectionSettings;
            _multiplexer = ConnectionMultiplexer.Connect(_connectionSettings.ConnectionString);
        }

        private IDatabase DB
        {
            get
            {
                IDatabase database = _multiplexer.GetDatabase(_connectionSettings.Db);
                return database;
            }

        }

        public void RemoveStartsWith(string key)
        {
            //AutoInvalidateCacheOutputAttribute and InvalidateCacheOutputAttribute uses this method.
            //TODO: delete by pattern 
            //EVAL "return redis.call('del', unpack(redis.call('keys', ARGV[1])))" 0 prefix:*
            //CAUTION : deleting key please use scan 
            DB.KeyDelete(key);
        }

        public T Get<T>(string key) where T : class
        {
            string redisValue = DB.StringGet(key);
            if (!string.IsNullOrEmpty(redisValue))
                return _jsonSerializer.DeserializeObject<T>(redisValue);

            return null;
        }

        public object Get(string key)
        {
            return DB.StringGet(key);
        }

        public void Remove(string key)
        {
            DB.KeyDelete(key);
        }

        public bool Contains(string key)
        {
            return DB.KeyExists(key);
        }

        public void Add(string key, object o, DateTimeOffset expiration, string dependsOnKey = null)
        {
            DB.StringSet(key, _jsonSerializer.SerializeObject(o), TimeSpan.FromTicks(expiration.DateTime.ToUniversalTime().Ticks));
        }

        public IEnumerable<string> AllKeys
        {
            get
            {
                //TODO: use SCAN to get keys
                return Enumerable.Empty<string>();
            }
        }
    }
}
