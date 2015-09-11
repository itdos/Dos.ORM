using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCache.Base
{
    public interface ICacheBase
    {
        bool Remove(string key);
        bool Set<T>(string key, T value);
        bool Set<T>(string key, T value, TimeSpan expiresIn);
        T Get<T>(string key);
    }
}
