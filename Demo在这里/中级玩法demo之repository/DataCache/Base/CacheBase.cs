using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataCache.Base
{
    public class CacheBase
    {
        /// <summary>
        /// 声明使用哪种缓存
        /// </summary>
        public static IISCacheBase Cache = new IISCacheBase();
        //public static RedisCacheBase Cache = new RedisCacheBase();
        public bool Remove(string key)
        {
            return Cache.Remove(key);
        }
        public bool Set<T>(string key, T value)
        {
            return Cache.Set(key, JsonConvert.SerializeObject(value));
        }
        public bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            return Cache.Set(key, JsonConvert.SerializeObject(value), expiresIn);
        }
        public T Get<T>(string key)
        {
            return Cache.Get<T>(key);
        }
    }
}
