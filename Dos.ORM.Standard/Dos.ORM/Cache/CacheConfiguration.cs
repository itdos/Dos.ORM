#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：
* Copyright(c) ITdos
* CLR 版本: 4.0.30319.18408
* 创 建 人：steven hu
* 电子邮箱：
* 官方网站：www.ITdos.com
* 创建日期：2010-2-10
* 文件描述：
******************************************************
* 修 改 人：ITdos
* 修改日期：2018-05-17
* 备注描述：
*******************************************************/
#endregion

using System.Collections.Generic;
using System.Configuration;
namespace Dos.ORM
{
    /// <summary>
    /// 缓存设置
    /// </summary>
    public class CacheConfiguration  
#if NET40
        : ConfigurationSection
#endif
    {
        /// <summary>
        /// 默认值
        /// </summary>
        private bool? enableCache = null;


        /// <summary>
        /// 是否开启缓存
        /// </summary>
#if NET40
        [ConfigurationProperty("enable")]
#endif
        public bool Enable
        {
            get
            {
                if (enableCache.HasValue)
                    return enableCache.Value;
#if NET40
                if (this["enable"] == null)
#else
                if (enableCache == null)
#endif

                    return false;
#if NET40
                return (bool)this["enable"];
#else
                return (bool)enableCache;
#endif

            }
            set { enableCache = value; }
        }


#if NET40
        /// <summary>
        /// 设置表缓存
        /// </summary>
        [ConfigurationProperty("entities", IsRequired = true, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(KeyValueConfigurationCollection))]
        public KeyValueConfigurationCollection Entities
        {
            get
            {
                return (KeyValueConfigurationCollection)this["entities"];
            }
        }
#else
        public Dictionary<string, string> Entities
        {
            get
            {
                return new Dictionary<string, string>();
            }
        }
#endif

    }
}
