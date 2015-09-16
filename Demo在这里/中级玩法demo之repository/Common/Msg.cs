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

namespace QzCRM.Common
{
    public class Msg
    {
        /// <summary>
        /// 参数错误
        /// </summary>
        public static readonly string ParamError = "参数不能为空！";
        /// <summary>
        /// 数据库受影响行数为0！
        /// </summary>
        public static readonly string Line0 = "数据库受影响行数为0！";
        /// <summary>
        /// 不存在的数据！
        /// </summary>
        public static readonly string NoExist = "不存在的数据！";
    }
}
