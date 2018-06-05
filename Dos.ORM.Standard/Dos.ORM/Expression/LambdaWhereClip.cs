#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：
* Copyright(c) ITdos
* CLR 版本: 4.0.30319.18408
* 创 建 人：steven hu
* 电子邮箱：
* 官方网站：www.ITdos.com
* 创建日期：2010/2/10
* 文件描述：
******************************************************
* 修 改 人：ITdos
* 修改日期：
* 备注描述：
*******************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Dos.ORM;
using Dos.ORM.Common;

namespace Dos.ORM
{
    /// <summary>
    /// 条件
    /// </summary>
    [Serializable]
    public class LambdaWhereClip<T>
    {
        /// <summary>
        /// All
        /// </summary>
        public readonly static LambdaWhereClip<T> All = new LambdaWhereClip<T>();

        #region 构造函数
        /// <summary>
        /// 
        /// </summary>
        public LambdaWhereClip() { }
        #endregion
    }
}
