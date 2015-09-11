using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCache.Base
{
    public class RedisConfig
    {
        /// <summary>
        /// 别名
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 默认数据库
        /// </summary>
        public int DataBase
        {
            get;
            set;
        }
        /// <summary>
        /// 主机列表
        /// </summary>
        public string Hosts
        {
            get;
            set;
        }
        /// <summary>
        /// 缓存默认过期时间
        /// </summary>
        public int CacheExpireMinutes
        {
            get;
            set;
        }
    }
}
