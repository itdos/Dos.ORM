#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：Biz_CarsInfoLogic
* Copyright(c) 青之软件
* CLR 版本: 4.0.30319.17929
* 创 建 人：ITdos
* 电子邮箱：admin@itdos.com
* 创建日期：2014/10/1 11:00:49
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
using System.Runtime.Serialization;

namespace Model.Base
{
    public abstract class Param
    {
        protected Param()
        {
        }
        public int? page { get; set; }
        public int? total { get; set; }
        public int? records { get; set; }
        public int? top { get; set; }
        /// <summary>
        /// 每页多少条
        /// </summary>
        public int? pageSize { get; set; }
        /// <summary>
        /// 第几页
        /// </summary>
        public int? pageIndex { get; set; }
    }
}
