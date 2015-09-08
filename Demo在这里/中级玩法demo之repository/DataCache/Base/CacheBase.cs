#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：Biz_CarsInfoLogic
* Copyright(c) 青之软件
* CLR 版本: 4.0.30319.17929
* 创 建 人：IT大师
* 电子邮箱：admin@itdos.com
* 创建日期：2014/10/1 11:00:49
* 文件描述：
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace DataCache
{
    public class CacheBase
    {
        static readonly IDatabase Cache = Connection.GetDatabase();
        private static ConnectionMultiplexer connection;
        private static ConnectionMultiplexer Connection
        {
            get
            {
                if (connection == null || !connection.IsConnected)
                {
                    connection = ConnectionMultiplexer.Connect("127.0.0.1:6379,ssl=false,password=");
                }
                return connection;
            }
        }
        public static bool Remove(string key)
        {
            return Cache.KeyDelete(key);
        }
        public static bool Set<T>(string key, T value)
        {
            return Cache.StringSet(key, JsonConvert.SerializeObject(value));
        }
        public static bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
           return Cache.StringSet(key, JsonConvert.SerializeObject(value), expiresIn);
        }
        public static T Get<T>(string key)
        {
            return JsonConvert.DeserializeObject<T>(Cache.StringGet(key));
        }
    }
}
