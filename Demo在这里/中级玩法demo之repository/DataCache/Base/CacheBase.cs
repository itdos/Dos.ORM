using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataCache.Base
{
    /// <summary>
    /// 可以根据情况使用不同的缓存机制。这里也可以做成反射+缓存机制来动态配置使用哪种缓存机制。
    /// </summary>
    public class CacheBase
    {
        public static IISCacheBase IisCache = new IISCacheBase();
        public static RedisCacheBase RedisCache = new RedisCacheBase();
        public static bool Remove(string key)
        {
            if (false)
            {
                return RedisCache.Remove(key);
            }
            return IisCache.Remove(key);
        }
        public static bool Set<T>(string key, T value)
        {
            return IisCache.Set(key, value);
        }
        public static bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            return IisCache.Set(key, value, expiresIn);
        }
        public static T Get<T>(string key)
        {
            return IisCache.Get<T>(key);
        }
    }
}
