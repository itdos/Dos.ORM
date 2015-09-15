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
using System.Linq.Expressions;
using System.Text;

namespace Dos.ORM
{
    /// <summary>
    /// 
    /// </summary>
    public class Where : WhereClipBuilder
    {

    }

    /// <summary>
    /// Where条件拼接，同WhereClipBuilder
    /// </summary>
    public class Where<T> : WhereClipBuilder
        where T : Entity
    {
        /// <summary>
        /// AND
        /// </summary>
        public void And(Expression<Func<T, bool>> lambdaWhere)
        {
            var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
            And(tempWhere);
        }
        /// <summary>
        /// AND
        /// </summary>
        public void And<T2>(Expression<Func<T,T2, bool>> lambdaWhere)
        {
            var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
            And(tempWhere);
        }
        public void And<T2, T3>(Expression<Func<T, T2, T3, bool>> lambdaWhere)
        {
            var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
            And(tempWhere);
        }
        public void And<T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> lambdaWhere)
        {
            var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
            And(tempWhere);
        }
        //public void And<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> lambdaWhere)
        //{
        //    var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
        //    And(tempWhere);
        //}
        //public void And<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> lambdaWhere)
        //{
        //    var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
        //    And(tempWhere);
        //}
        /// <summary>
        /// Or
        /// </summary>
        public void Or(Expression<Func<T, bool>> lambdaWhere)
        {
            var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
            Or(tempWhere);
        }
        public void Or<T2>(Expression<Func<T, T2, bool>> lambdaWhere)
        {
            var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
            Or(tempWhere);
        }
        public void Or<T2, T3>(Expression<Func<T, T2, T3, bool>> lambdaWhere)
        {
            var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
            Or(tempWhere);
        }
        public void Or<T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> lambdaWhere)
        {
            var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
            Or(tempWhere);
        }
        //public void Or<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> lambdaWhere)
        //{
        //    var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
        //    Or(tempWhere);
        //}
        //public void Or<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> lambdaWhere)
        //{
        //    var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
        //    Or(tempWhere);
        //}
    }
    /// <summary>
    /// Where条件拼接，同Where类
    /// </summary>
    [Obsolete("建议使用Where类替代WhereClip类。使用示例：var W = new Where<T>(); W.And(d=>d.ID == 1);")]
    public class WhereClipBuilder<T> : WhereClipBuilder
        where T : Entity
    {
        /// <summary>
        /// AND
        /// </summary>
        public void And(Expression<Func<T, bool>> lambdaWhere)
        {
            var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
            And(tempWhere);
        }
        /// <summary>
        /// Or
        /// </summary>
        public void Or(Expression<Func<T, bool>> lambdaWhere)
        {
            var tempWhere = ExpressionToClip<T>.ToWhereClip(lambdaWhere);
            Or(tempWhere);
        }
    }

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
