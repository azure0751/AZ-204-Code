using StackExchange.Redis;

namespace RedisCacheBlobReaderDemo.Services
{
    public class RedisCacheService
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisCacheService()
        {
        }

        public RedisCacheService(IConfiguration configuration)
        {
            _redis = ConnectionMultiplexer.Connect(configuration["RedisCache:ConnectionString"]);
            _database = _redis.GetDatabase();
        }

        public RedisCacheService(string connectionString)
        {
            _redis = ConnectionMultiplexer.Connect(connectionString);
            _database = _redis.GetDatabase();
        }

        public async Task SetCacheAsync(string key, string value, TimeSpan? expiration = null)
        {
            // Set the cache with an expiration time
            await _database.StringSetAsync(key, value, expiration);
        }

        public async Task<string> GetCacheAsync(string key)
        {
            return await _database.StringGetAsync(key);
        }
    }
}