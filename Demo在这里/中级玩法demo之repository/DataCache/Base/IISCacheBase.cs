using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dos.Common;
using Dos.ORM;
using Newtonsoft.Json;

namespace DataCache.Base
{
    public class IISCacheBase : ICacheBase
    {
        public bool Remove(string key)
        {
            CacheHelper.Remove(key);
            return true;
        }

        public bool Set<T>(string key, T value)
        {
            CacheHelper.Set(key, value);
            return true;
        }

        public bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            CacheHelper.Set(key, value, expiresIn.Seconds);
            return true;
        }

        public T Get<T>(string key)
        {
            return JsonConvert.DeserializeObject<T>(CacheHelper.Get(key).ToString());
        }
    }
}
