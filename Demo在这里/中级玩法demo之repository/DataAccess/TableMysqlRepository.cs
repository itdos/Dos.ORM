#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：Sys_BankCardInfoRepository
* Copyright(c) 青之软件
* CLR 版本: 4.0.30319.17929
* 创 建 人：向娟
* 电子邮箱：
* 创建日期：2014/10/28 11:00:49
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
using DataAccess.Entities;
using Common;

namespace DataAccess
{
    /// <summary>
    /// 数据库处理层。多表联查、复杂的Dos.ORM写法都可以丢到这层来写。
    /// </summary>
    public class TableMysqlRepository : Repository<TestTable>
    {
        /// <summary>
        /// 
        /// </summary>
        public TableMysqlRepository()
        {
            
        }
    }
}
