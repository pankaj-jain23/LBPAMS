using EAMS_ACore.IExternal;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace EAMS_BLL.ExternalServices
{
    public class CacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;

        public CacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        // Get Redis database instance
        private IDatabase GetDatabase()
        {
            return _redis.GetDatabase();
        }

        // Get data from Redis cache asynchronously by key
        public async Task<T> GetDataAsync<T>(string key)
        {
            try
            {
                var db = GetDatabase();
                var value = await db.StringGetAsync(key);

                if (!string.IsNullOrEmpty(value))
                {
                    return JsonConvert.DeserializeObject<T>(value);
                }
            }
            catch (RedisException ex)
            {
                // Handle Redis-specific exceptions or log the error
                // Log(ex); // Implement logging
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                // Log(ex); // Implement logging
            }

            return default(T);
        }

        // Set data into Redis cache asynchronously with an expiration time
        public async Task<bool> SetDataAsync<T>(string key, T value, DateTimeOffset expirationTime)
        {
            try
            {
                var db = GetDatabase();
                TimeSpan expiryTime = expirationTime - DateTimeOffset.Now;
                var isSet = await db.StringSetAsync(key, JsonConvert.SerializeObject(value), expiryTime);
                return isSet;
            }
            catch (RedisException ex)
            {
                // Handle Redis-specific exceptions or log the error
                // Log(ex); // Implement logging
                return false;
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                // Log(ex); // Implement logging
                return false;
            }
        }

        // Remove data from Redis cache asynchronously by key
        public async Task<bool> RemoveDataAsync(string key)
        {
            try
            {
                var db = GetDatabase();
                bool isKeyExist = await db.KeyExistsAsync(key);

                if (isKeyExist)
                {
                    return await db.KeyDeleteAsync(key);
                }
            }
            catch (RedisException ex)
            {
                // Handle Redis-specific exceptions or log the error
                // Log(ex); // Implement logging
                return false;
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                // Log(ex); // Implement logging
                return false;
            }

            return false;
        }
    }
}
