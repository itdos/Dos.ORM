#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：Biz_CarsInfoLogic
* Copyright(c) 青之软件
* CLR 版本: 4.0.30319.17929
* 创 建 人：周浩
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

namespace DataAccess.Entities.Base
{
    [DataContract(IsReference = true)]
    public abstract class Param
    {
        /// <summary>
        /// 前多少条
        /// </summary>
        [DataMember]
        public int? _Top { get; set; }
        /// <summary>
        /// 每页多少条
        /// </summary>
        [DataMember]
        public int? _PageSize { get; set; }
        /// <summary>
        /// 第几页。1开始计数
        /// </summary>
        [DataMember]
        public int? _PageIndex { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string _OrderBy { get; set; }
        /// <summary>
        /// 排序方式
        /// </summary>
        public string _OrderByType { get; set; }
    }
}
