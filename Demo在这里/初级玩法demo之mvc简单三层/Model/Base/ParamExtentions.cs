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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Model.Base
{
    public class TestTableParam : Param
    {
        public Guid? Id { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Name { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string IDNumber { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string MobilePhone { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SearchName { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SearchIDNumber { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SearchMobilePhone { get; set; }
    }
}