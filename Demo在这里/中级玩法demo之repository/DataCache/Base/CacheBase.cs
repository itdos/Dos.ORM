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
using Common;
using ServiceStack.Redis;

namespace DataCache
{
    public class CacheBase
    {
        public static PooledRedisClientManager CreateManager(
            string[] readWriteHosts, string[] readOnlyHosts)
        {
            return new PooledRedisClientManager(readWriteHosts, readOnlyHosts,
                new RedisClientManagerConfig
                {
                    MaxWritePoolSize = readWriteHosts.Length * 2,
                    MaxReadPoolSize = readOnlyHosts.Length * 2,
                    AutoStart = true,
                });
        }
        private static readonly string RedisKeyPrefix = ConfigurationManager.AppSettings["RedisKeyPrefix"];
        private static readonly string RedisClientPwd = ConfigurationManager.AppSettings["RedisClientPwd"];
        private static readonly string RedisClientHost = ConfigurationManager.AppSettings["RedisClientHost"];
        private static readonly string RedisClientPort = ConfigurationManager.AppSettings["RedisClientPort"];
        private static readonly PooledRedisClientManager Prcm = CreateManager(new string[]
        {
            RedisClientPwd + "@" +RedisClientHost + ":" + RedisClientPort,
            RedisClientPwd + "@" +RedisClientHost + ":" + RedisClientPort,
            RedisClientPwd + "@" +RedisClientHost + ":" + RedisClientPort,
            RedisClientPwd + "@" +RedisClientHost + ":" + RedisClientPort,
            RedisClientPwd + "@" +RedisClientHost + ":" + RedisClientPort,
            RedisClientPwd + "@" +RedisClientHost + ":" + RedisClientPort,
            RedisClientPwd + "@" +RedisClientHost + ":" + RedisClientPort,
            RedisClientPwd + "@" +RedisClientHost + ":" + RedisClientPort,
            RedisClientPwd + "@" +RedisClientHost + ":" + RedisClientPort,
            RedisClientPwd + "@" +RedisClientHost + ":" + RedisClientPort
        }, new string[]
        {
            RedisClientPwd + "@" +RedisClientHost + ":" + RedisClientPort,
            RedisClientPwd + "@" +RedisClientHost + ":" + RedisClientPort,
            RedisClientPwd + "@" +RedisClientHost + ":" + RedisClientPort,
            RedisClientPwd + "@" +RedisClientHost + ":" + RedisClientPort,
            RedisClientPwd + "@" +RedisClientHost + ":" + RedisClientPort,
            RedisClientPwd + "@" +RedisClientHost + ":" + RedisClientPort,
            RedisClientPwd + "@" +RedisClientHost + ":" + RedisClientPort,
            RedisClientPwd + "@" +RedisClientHost + ":" + RedisClientPort,
            RedisClientPwd + "@" +RedisClientHost + ":" + RedisClientPort,
            RedisClientPwd + "@" +RedisClientHost + ":" + RedisClientPort
        });
        public static void Save()
        {
            using (var redis = Prcm.GetClient())
            {
                try
                {

                    //如果同步保存正在执行，再次调用会抛出异常
                    redis.Save();
                }
                catch (Exception)
                {

                }
            }
        }
        public static List<string> GetAllKeys()
        {
            using (var redis = Prcm.GetClient())
            {
                return redis.GetAllKeys();
            }
        }
        public static void SaveAsync()
        {
            using (var redis = Prcm.GetClient())
            {
                try
                {
                    //如果异步保存正在执行，再次调用会抛出异常
                    redis.SaveAsync();
                }
                catch (Exception)
                {

                }
            }
        }
        public static bool FlushAll()
        {
            using (var redis = Prcm.GetClient())
            {
                redis.FlushAll();
                return true;
            }
        }
        public static bool Remove(string key)
        {
            using (var redis = Prcm.GetClient())
            {
                var result = redis.Remove(RedisKeyPrefix + key);
                if (result)
                {
                    return true;
                }
                return false;
            }
        }
        public static bool Set<T>(string key, T value)
        {
            using (var redis = Prcm.GetClient())
            {
                var result = redis.Set(RedisKeyPrefix + key, value, DateTime.Now.AddDays(30));
                if (result)
                {
                    return true;
                }
                return false;
            }
        }
        public static bool Set<T>(string key, T value, DateTime expiresAt)
        {
            using (var redis = Prcm.GetClient())
            {
                var result = redis.Set(RedisKeyPrefix + key, value, expiresAt);
                if (result)
                {
                    return true;
                }
                return false;
            }
        }
        public static bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            using (var redis = Prcm.GetClient())
            {
                var result = redis.Set(RedisKeyPrefix + key, value, expiresIn);
                if (result)
                {
                    return true;
                }
                return false;
            }
        }
        public static T Get<T>(string key)
        {
            using (var redis = Prcm.GetClient())
            {
                return redis.Get<T>(RedisKeyPrefix + key);
            }
        }
        /// <summary>
        /// 给Key的值++
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long IncrementValue(string key)
        {
            using (var redis = Prcm.GetClient())
            {
                return redis.IncrementValue(RedisKeyPrefix + key);
            }
        }
        /// <summary>
        /// 给Key的值加上 Count
        /// </summary>
        /// <param name="key"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static long IncrementValueBy(string key, int count)
        {
            using (var redis = Prcm.GetClient())
            {
                return redis.IncrementValueBy(RedisKeyPrefix + key, count);
            }
        }
    }
}
