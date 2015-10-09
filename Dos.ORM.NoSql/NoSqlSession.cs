using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dos.ORM.NoSql;

namespace Dos.ORM.NoSql
{
    /// <summary>
    /// 
    /// </summary>
    public class NoSqlSession
    {
        /// <summary>
        /// NoSql数据库类型
        /// </summary>
        public NoSqlType NoSqlType;
        /// <summary>
        /// 缓存对象
        /// </summary>
        private ICache Cache;
        /// <summary>
        /// 构造函数
        /// </summary>
        public NoSqlSession()
        {
            var type = System.Configuration.ConfigurationManager.AppSettings["NoSqlType"];
            if (string.IsNullOrWhiteSpace(type))
            {
                throw new Exception("请在AppSetting中配置<add key=\"NoSqlType\" value=\"IIS/Redis/MongoDB/Memcache\" />");
            }

            switch (type.ToLower())
            {
                case "redis":
                    NoSqlType = NoSqlType.Redis;
                    Cache = new Redis();
                    break;
                case "iis":
                    NoSqlType = NoSqlType.IIS;
                    Cache = new IIS();
                    break;
                default:
                    throw new Exception("暂时不支持的NoSql数据库！");
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public NoSqlSession(NoSqlType noSqlType,string host = "",string port = "",string pwd = "")
        {
            switch (noSqlType)
            {
                case NoSqlType.Redis:
                    NoSqlType = NoSqlType.Redis;
                    Cache = new Redis(host, port, pwd);
                    break;
                case NoSqlType.IIS:
                    NoSqlType = NoSqlType.IIS;
                    Cache = new IIS();
                    break;
                default:
                    throw new Exception("暂时不支持的NoSql数据库！");
            }
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return Cache.Remove(key);
        }
        /// <summary>
        /// 新增/覆盖 缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value)
        {
            return Cache.Set(key, value);
        }
        /// <summary>
        /// 新增/覆盖 缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresIn"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            return Cache.Set(key, value, expiresIn);
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return Cache.Get<T>(key);
        }
    }
}
