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
using System.Collections.Concurrent;
using System.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Dos.ORM.NoSql
{
    /// <summary>
    /// Redis缓存。需要在AppSetting中配置：RedisHost、RedisPort
    /// </summary>
    public class Redis : ICache
    {
        private static RedisConfig _conf;
        /// <summary>
        /// 
        /// </summary>
        public static RedisConfig Conf
        {
            get
            {
                if (_conf == null)
                {
                    return new RedisConfig()
                    {
                        Hosts = ConfigurationManager.AppSettings["RedisHost"]
                        + ":" +
                        ConfigurationManager.AppSettings["RedisPort"]
                    };
                }
                return _conf;
            }
            set { _conf = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Redis()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public Redis(string host,string port,string pwd)
        {
            Conf = new RedisConfig()
            {
                Hosts = host
                + ":" +
                port
            };
        }

        static ConcurrentDictionary<string, ConnectionMultiplexer> _multiplexers = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        /// <summary>
        /// 获取client
        /// </summary>
        /// <returns></returns>
        private static ConnectionMultiplexer GetMultiplexer(RedisConfig config)
        {
            ConnectionMultiplexer multiplexer = null;
            if (!_multiplexers.TryGetValue(config.Hosts, out multiplexer))
            {
                var redisconf = ConfigurationOptions.Parse(config.Hosts);
                redisconf.AllowAdmin = true;
                redisconf.ConnectRetry = 2;
                multiplexer = ConnectionMultiplexer.Connect(redisconf);
                _multiplexers.TryAdd(config.Hosts, multiplexer);
            }
            return multiplexer;
        }
        /// <summary>
        /// redis连接端
        /// </summary>
        public ConnectionMultiplexer conn
        {
            get
            {
                return GetMultiplexer(Conf); ;
            }
        }
        public IDatabase Cache
        {
            get
            {
                return conn.GetDatabase(Conf.DataBase);
            }
        }
        public bool Remove(string key)
        {
            return Cache.KeyDelete(key);
        }
        public bool Set<T>(string key, T value)
        {
            return Cache.StringSet(key, JsonConvert.SerializeObject(value));
        }
        public bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            return Cache.StringSet(key, JsonConvert.SerializeObject(value), expiresIn);
        }
        public T Get<T>(string key)
        {
            var result = Cache.StringGet(key);
            if (result != RedisValue.Null)
            {
                return JsonConvert.DeserializeObject<T>(result);
            }
            return default(T);
        }
    }
}
