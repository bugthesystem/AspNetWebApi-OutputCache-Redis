using System;
using System.Collections.Generic;
using System.Linq;
using Jil;
using StackExchange.Redis;
using WebApi.OutputCache.Core.Cache;

namespace WebAPI.OutputCache.Redis
{
    public class RedisOutputCache : IApiOutputCache
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IRedisConnectionSettings _connectionSettings;
        private readonly IConnectionMultiplexer _multiplexer;
        readonly Options _options;

        public RedisOutputCache(IJsonSerializer jsonSerializer, IRedisConnectionSettings connectionSettings)
        {
            _jsonSerializer = jsonSerializer;
            _connectionSettings = connectionSettings;
            _multiplexer = ConnectionMultiplexer.Connect(_connectionSettings.ConnectionString);
            _options = null;//TODO:
        }

        private IDatabase DB => _multiplexer.GetDatabase(_connectionSettings.Db);

        public void RemoveStartsWith(string key)
        {
            //AutoInvalidateCacheOutputAttribute and InvalidateCacheOutputAttribute uses this method.
            //TODO: delete by pattern 
            //EVAL "return redis.call('del', unpack(redis.call('keys', ARGV[1])))" 0 prefix:*
            //CAUTION : while deleting keys please use scan 
            DB.KeyDelete(key);
        }

        public T Get<T>(string key) where T : class
        {
            string redisValue = DB.StringGet(key);

            if (!string.IsNullOrEmpty(redisValue))
                return _jsonSerializer.DeserializeObject<T>(redisValue, _options);

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
            TimeSpan timeSpan = expiration.DateTime.Subtract(DateTime.Now);
            DB.StringSet(key, _jsonSerializer.SerializeObject(o, _options), timeSpan);
        }

        //TODO: use SCAN to get keys
        public IEnumerable<string> AllKeys => Enumerable.Empty<string>();
    }
}
