#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：
* Copyright(c) 青之软件
* CLR 版本: 4.0.30319.17929
* 创 建 人：周浩
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

namespace Dos.ORM.NoSql
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
