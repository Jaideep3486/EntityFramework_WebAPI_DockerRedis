
using StackExchange.Redis;
using System.Text.Json;

namespace webAPI2.Models
{
    public class CachedService : ICachedService
    {

        private IDatabase _db;

        public CachedService()
        {
            try
            {
                var redis = ConnectionMultiplexer.Connect("localhost:6379");
                _db = redis.GetDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine("There is an Exception !!!!!!!!!!!!!!!!!!!!"+ex);

            }
            
        }
        public T GetData<T>(string Key)
        {
            try { 
            var value=_db.StringGet(Key);
            if (!string.IsNullOrEmpty(value))
                return JsonSerializer.Deserialize<T>(value);
            }
            catch (Exception ex) { Console.WriteLine("There is an Exception !!!!!!!!!!!!!!!!!!!!" + ex);
                
            }
            return default;

        }

        public object Removedata(string key)
        {
            var _exist=_db.KeyExists(key);

            if(_exist)
                return _db.KeyDelete(key);
            return false;
        }

        public bool SetData<T>(string key, T Value, DateTimeOffset expirationTime)
        {
            var expirytime = expirationTime.DateTime.Subtract(DateTime.Now);
            var isSet = _db.StringSet(key,JsonSerializer.Serialize(Value),expirytime);
            return isSet;
        }
    }
}
