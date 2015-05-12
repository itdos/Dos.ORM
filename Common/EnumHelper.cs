#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：ExpToGroupByClip
* Copyright(c) IT大师
* CLR 版本: 4.0.30319.18408
* 创 建 人：ITdos
* 电子邮箱：admin@itdos.com
* 官方网站：www.ITdos.com
* 创建日期：2015/5/10 10:54:32
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

namespace Dos.ORM.Common
{
    public enum FieldType
    {
        Normal,
        HasOne,
        HasMany,
        BelongsTo,
        HasAndBelongsToMany,
        LazyLoad
    }

    public enum ColumnFunction
    {
        None,
        ToLower,
        ToUpper,
    }
    /// <summary>
    /// 连接类型
    /// </summary>
    public enum JoinType : byte
    {
        /// <summary>
        /// InnerJoin
        /// </summary>
        InnerJoin,
        /// <summary>
        /// LeftJoin
        /// </summary>
        LeftJoin,
        /// <summary>
        /// RightJoin
        /// </summary>
        RightJoin,
        /// <summary>
        /// CrossJoin
        /// </summary>
        CrossJoin,
        /// <summary>
        /// FullJoin
        /// </summary>
        FullJoin
    }
}
