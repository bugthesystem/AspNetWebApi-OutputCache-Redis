# ASP.NET Web API CacheOutput - Redis

Redis `provider` for AspNetWebApi-OutputCache
Implementation of IApiOutputCache for Redis

## Sample

You can register your implementation using a handy GlobalConfiguration extension method:

```csharp
var _connectionSettings = new RedisConnectionSettings { ConnectionString = "localhost:6379", Db = 2 };
var mycache = new RedisOutputCache(new JsonSerializer(), _connectionSettings);

//instance
configuration.CacheOutputConfiguration().RegisterCacheOutputProvider(() => mycache);
```
