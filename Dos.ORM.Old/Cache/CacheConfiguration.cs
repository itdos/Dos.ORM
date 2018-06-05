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
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using System.Configuration;
namespace Dos.ORM
{


    /// <summary>
    /// 缓存设置
    /// </summary>
    public class CacheConfiguration : ConfigurationSection
    {
        /// <summary>
        /// 默认值
        /// </summary>
        private bool? enableCache = null;


        /// <summary>
        /// 是否开启缓存
        /// </summary>
        [ConfigurationProperty("enable")]
        public bool Enable
        {
            get
            {
                if (enableCache.HasValue)
                    return enableCache.Value;

                if (this["enable"] == null)
                    return false;

                return (bool)this["enable"];

            }
            set { enableCache = value; }
        }

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
    }
}
