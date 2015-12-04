/*************************************************************************
 * 
 * Hxj.Data
 * 
 * 2010-2-10
 * 
 * steven hu   
 *  
 * Support: http://www.cnblogs.com/huxj
 *   
 * 
 * Change History:
 * 
 * 
**************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Caching;

namespace Dos.ORM
{
    /// <summary>
    /// 简单缓存处理类
    /// </summary>
    public class Cache
    {

        /// <summary>
        /// 默认cache实例
        /// </summary>
        public static Dos.ORM.Cache Default = new Cache();

        /// <summary>
        /// cache
        /// </summary>
        private static volatile System.Web.Caching.Cache hxjCache = System.Web.HttpRuntime.Cache;

        /// <summary>
        /// timeout 600秒
        /// </summary>
        private int _timeOut = 600;

        #region 构造函数

        /// <summary>
        /// 
        /// </summary>
        public Cache() { }

        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="timeOut"></param>
        public Cache(int timeOut)
        {
            this._timeOut = timeOut;
        }

        #endregion

        ///// <summary>
        ///// lock object
        ///// </summary>
        //private static object lockobj = new object();

        /// <summary>
        /// time out (seconds)   
        /// 
        /// <![CDATA[if timeout <=0,the cache expiration time is maxvalue]]>
        /// 
        /// </summary>
        public int TimeOut
        {
            set { _timeOut = value; }
            get { return _timeOut; }
        }


        /// <summary>
        /// 添加缓存 (绝对有效期)
        /// </summary>
        /// <param name="cacheKey">缓存键值</param>
        /// <param name="cacheValue">缓存内容</param>
        public void AddCache(string cacheKey, object cacheValue)
        {
            AddCache(cacheKey, cacheValue, TimeOut);
        }

        /// <summary>
        /// 添加缓存 (绝对有效期)
        /// </summary>
        /// <param name="cacheKey">缓存键值</param>
        /// <param name="cacheValue">缓存内容</param>
        /// <param name="timeout">绝对有效期（单位: 秒）</param>
        public void AddCache(string cacheKey, object cacheValue, int timeout)
        {
            
            if (string.IsNullOrEmpty(cacheKey))
            {
                return;
            }

            if (null == cacheValue)
            {
                RemoveCache(cacheKey);
                return;
            }

            CacheItemRemovedCallback callBack = new CacheItemRemovedCallback(onRemove);

            if (timeout <= 0)
            {
                hxjCache.Insert(cacheKey, cacheValue, null, DateTime.MaxValue, TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, callBack);
            }
            else
            {
                hxjCache.Insert(cacheKey, cacheValue, null, DateTime.Now.AddSeconds(timeout), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, callBack);
            }
        }

        /// <summary>
        /// 添加缓存 (相对有效期)
        /// </summary>
        /// <param name="cacheKey">缓存键值</param>
        /// <param name="cacheValue">缓存内容</param>
        public void AddCacheSlidingExpiration(string cacheKey, object cacheValue)
        {
            AddCacheSlidingExpiration(cacheKey, cacheValue, TimeOut);
        }

        /// <summary>
        /// 添加缓存 (相对有效期)
        /// </summary>
        /// <param name="cacheKey">缓存键值</param>
        /// <param name="cacheValue">缓存内容</param>
        /// <param name="timeout">相对过期时间 (单位: 秒)</param>
        public void AddCacheSlidingExpiration(string cacheKey, object cacheValue, int timeout)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                return;
            }

            if (null == cacheValue)
            {
                RemoveCache(cacheKey);
                return;
            }

            CacheItemRemovedCallback callBack = new CacheItemRemovedCallback(onRemove);

            if (timeout <= 0)
            {
                hxjCache.Insert(cacheKey, cacheValue, null, DateTime.MaxValue, TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, callBack);
            }
            else
            {
                hxjCache.Insert(cacheKey, cacheValue, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(timeout), System.Web.Caching.CacheItemPriority.High, callBack);
            }
        }


        /// <summary>
        /// 添加缓存 (文件依赖)
        /// </summary>
        /// <param name="cacheKey">缓存键值</param>
        /// <param name="cacheValue">缓存内容</param>
        /// <param name="filenames">缓存依赖的文件或目录</param>
        public void AddCacheFilesDependency(string cacheKey, object cacheValue, params string[] filenames)
        {            
            CacheDependency dep = new CacheDependency(filenames, DateTime.Now);

            AddCacheDependency(cacheKey, cacheValue, TimeOut, dep);
        }

        /// <summary>
        /// 添加缓存 (文件依赖)
        /// </summary>
        /// <param name="cacheKey">缓存键值</param>
        /// <param name="cacheValue">缓存内容</param>
        /// <param name="timeout">绝对过期时间 （单位：秒）</param>
        /// <param name="dep">缓存依赖</param>
        public void AddCacheDependency(string cacheKey, object cacheValue, int timeout, CacheDependency dep)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                return;
            }

            if (null == cacheValue)
            {
                RemoveCache(cacheKey);
                return;
            }

            CacheItemRemovedCallback callBack = new CacheItemRemovedCallback(onRemove);

            if (timeout <= 0)
            {
                hxjCache.Insert(cacheKey, cacheValue, dep, DateTime.MaxValue, TimeSpan.Zero, System.Web.Caching.CacheItemPriority.High, callBack);
            }
            else
            {
                hxjCache.Insert(cacheKey, cacheValue, dep, System.DateTime.Now.AddSeconds(timeout), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, callBack);
            }
        }


        /// <summary>
        /// 添加缓存 (一组键值依赖)
        /// </summary>
        /// <param name="cacheKey">缓存键值</param>
        /// <param name="cacheValue">缓存内容</param>
        /// <param name="cachekeys">一组缓存键，此改变缓存也失效</param>
        public void AddCacheKeysDependency(string cacheKey, object cacheValue, string[] cachekeys)
        {

            CacheDependency dep = new CacheDependency(null, cachekeys, DateTime.Now);

            AddCacheDependency(cacheKey, cacheValue, TimeOut, dep);
        }



        /// <summary>
        /// 缓存删除的委托实例
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="reason"></param>
        private void onRemove(string key, object val, CacheItemRemovedReason reason)
        {
            //switch (reason)
            //{
            //    case CacheItemRemovedReason.DependencyChanged:
            //        break;
            //    case CacheItemRemovedReason.Expired:
            //        break;
            //    case CacheItemRemovedReason.Removed:
            //        break;
            //    case CacheItemRemovedReason.Underused:
            //        break;
            //    default: break;
            //}

            //do something: write log  ext.

        }


        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="cacheKey">缓存键值</param>
        public void RemoveCache(string cacheKey)
        {
            if (!string.IsNullOrEmpty(cacheKey))
                hxjCache.Remove(cacheKey);
        }


        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="cacheKey">对象的关键字</param>
        /// <returns></returns>
        public object GetCache(string cacheKey)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                return null;
            }
            return hxjCache.Get(cacheKey);
        }


        /// <summary>
        /// 获取缓存数量
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            return hxjCache.Count;
        }


        /// <summary>
        /// 返回缓存键值列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetCacheKeys()
        {
            List<string> cacheKeys = new List<string>();
            IDictionaryEnumerator cacheEnum = hxjCache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                cacheKeys.Add(cacheEnum.Key.ToString());
            }
            return cacheKeys;
        }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public void ClearAll()
        {
            List<string> cacheKeys = GetCacheKeys();
            foreach (string cacheKey in cacheKeys)
            {
                RemoveCache(cacheKey);
            }
        }

    }
}
