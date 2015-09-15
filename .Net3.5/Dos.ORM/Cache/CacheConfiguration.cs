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
