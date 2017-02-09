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
        private readonly Lazy<IConnectionMultiplexer> _multiplexer;
        readonly Options _options;

        public RedisOutputCache(IJsonSerializer jsonSerializer, IRedisConnectionSettings connectionSettings)
        {
            _jsonSerializer = jsonSerializer;
            _connectionSettings = connectionSettings;
            _multiplexer = new Lazy<IConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(_connectionSettings.ConnectionString));

            _options = null;//TODO:
        }

        private IDatabase Db => _multiplexer.Value.GetDatabase(_connectionSettings.Db);

        public void RemoveStartsWith(string key)
        {
            //AutoInvalidateCacheOutputAttribute and InvalidateCacheOutputAttribute uses this method.
            //TODO: delete by pattern 
            //EVAL "return redis.call('del', unpack(redis.call('keys', ARGV[1])))" 0 prefix:*
            //CAUTION : while deleting keys please use scan 
            Db.KeyDelete(key);
        }

        public T Get<T>(string key) where T : class
        {
            string redisValue = Db.StringGet(key);

            if (!string.IsNullOrEmpty(redisValue))
                return _jsonSerializer.DeserializeObject<T>(redisValue, _options);

            return null;
        }

        public object Get(string key)
        {
            return Db.StringGet(key);
        }

        public void Remove(string key)
        {
            Db.KeyDelete(key);
        }

        public bool Contains(string key)
        {
            return Db.KeyExists(key);
        }

        public void Add(string key, object o, DateTimeOffset expiration, string dependsOnKey = null)
        {
            TimeSpan timeSpan = expiration.DateTime.Subtract(DateTime.Now);
            Db.StringSet(key, _jsonSerializer.SerializeObject(o, _options), timeSpan);
        }

        //TODO: use SCAN to get keys
        public IEnumerable<string> AllKeys => Enumerable.Empty<string>();
    }
}
