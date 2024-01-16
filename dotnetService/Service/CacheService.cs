using System;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace dotnetService.Service
{
    public class CacheService : ICacheService
    {
        private IDatabase _db;

        public CacheService(IConfiguration configuration)
        {
            _db = ConnectionMultiplexer.Connect(configuration.GetValue<string>("RedisURL")).GetDatabase();
        }

        public T GetData<T>(string key)
        {
            var value = _db.StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default;
        }
        public bool SetData<T>(string key, T value, DateTimeOffset? expirationTime = null)
        {
            var isSet = _db.StringSet(key, JsonConvert.SerializeObject(value));
            if (expirationTime != null)
            {
                TimeSpan expiryTime = expirationTime.Value.DateTime.Subtract(DateTime.Now);
                isSet = _db.StringSet(key, JsonConvert.SerializeObject(value), expiryTime);
            }

            return isSet;
        }
        public object RemoveData(string key)
        {
            bool _isKeyExist = _db.KeyExists(key);
            if (_isKeyExist == true)
            {
                return _db.KeyDelete(key);
            }
            return false;
        }
    }
}
