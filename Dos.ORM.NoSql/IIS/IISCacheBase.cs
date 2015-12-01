#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：
* Copyright(c) 青之软件
* CLR 版本: 4.0.30319.17929
* 创 建 人：IT大师
* 电子邮箱：admin@itdos.com
* 创建日期：2015/09/10 14:08:52
* 文件描述：
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Dos.ORM.NoSql
{
    /// <summary>
    /// IIS缓存。无需任何配置。
    /// </summary>
    public class IIS : ICache
    {
        public bool Remove(string key)
        {
             Dos.Common.CacheHelper.Remove(key);
            return true;
        }
        public bool Set<T>(string key, T value)
        {
            Dos.Common.CacheHelper.Set(key, JsonConvert.SerializeObject(value));
            return true;
        }

        public bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            Dos.Common.CacheHelper.Set(key, JsonConvert.SerializeObject(value), expiresIn.Seconds);
            return true;
        }
        public T Get<T>(string key)
        {
            var result = Dos.Common.CacheHelper.Get(key);
            if (result != null)
            {
                return JsonConvert.DeserializeObject<T>(Dos.Common.CacheHelper.Get(key).ToString());
            }
            return default(T);
        }
    }
}
