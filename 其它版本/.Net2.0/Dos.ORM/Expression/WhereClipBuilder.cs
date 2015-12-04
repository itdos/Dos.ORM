#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：
* Copyright(c) IT大师
* CLR 版本: 4.0.30319.18408
* 创 建 人：ITdos
* 电子邮箱：admin@itdos.com
* 官方网站：www.ITdos.com
* 创建日期：2015/6/3 10:54:32
* 文件描述：
******************************************************
* 修 改 人：
* 修改日期：
* 备注描述：
*******************************************************/
#endregion



using System;
using System.Collections.Generic;

using System.Text;

namespace Dos.ORM
{
    /// <summary>
    /// WhereClipBuilder
    /// </summary>
    public class WhereClipBuilder
    {
        /// <summary>
        /// 条件字符串
        /// </summary>
        private StringBuilder expressionStringBuilder = new StringBuilder();

        /// <summary>
        /// 条件参数
        /// </summary>
        private List<Parameter> parameters = new List<Parameter>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public WhereClipBuilder()
        { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="where"></param>
        public WhereClipBuilder(WhereClip where)
        {
            expressionStringBuilder.Append(where.ToString());

            parameters.AddRange(where.Parameters);

        }

        

        /// <summary>
        /// AND
        /// </summary>
        /// <param name="where"></param>
        public void And(WhereClip where)
        {
            if (WhereClip.IsNullOrEmpty(where))
                return;


            if (expressionStringBuilder.Length > 0)
            {
                expressionStringBuilder.Append(" AND ");
                expressionStringBuilder.Append(where.ToString());
                expressionStringBuilder.Append(")");
                expressionStringBuilder.Insert(0, "(");
            }
            else
            {
                expressionStringBuilder.Append(where.ToString());
            }

            parameters.AddRange(where.Parameters);
        }

        /// <summary>
        /// Or
        /// </summary>
        /// <param name="where"></param>
        public void Or(WhereClip where)
        {
            if (WhereClip.IsNullOrEmpty(where))
                return;


            if (expressionStringBuilder.Length > 0)
            {
                expressionStringBuilder.Append(" OR ");
                expressionStringBuilder.Append(where.ToString());
                expressionStringBuilder.Append(")");
                expressionStringBuilder.Insert(0, "(");
            }
            else
            {
                expressionStringBuilder.Append(where.ToString());
            }
           

            parameters.AddRange(where.Parameters);
        }


        /// <summary>
        /// 转换成WhereClip
        /// </summary>
        /// <returns></returns>
        public WhereClip ToWhereClip()
        {
            return new WhereClip(expressionStringBuilder.ToString(), parameters.ToArray());
        }
    }
}
