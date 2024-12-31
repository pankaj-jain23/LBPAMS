using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_ACore.IExternal
{
    public interface ICacheService
    {
        Task<T> GetDataAsync<T>(string key);

        // Set data into Redis cache asynchronously with an expiration time
        Task<bool> SetDataAsync<T>(string key, T value, DateTimeOffset expirationTime);

        // Remove data from Redis cache asynchronously by key
        Task<bool> RemoveDataAsync(string key);
    }
}
