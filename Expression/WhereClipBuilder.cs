/*************************************************************************
 * 
 * Filename :WhereClipBuilder
 * 
 * steven hu   2010/8/3 15:08:51
 *  
 * http://www.cnblogs.com/huxj
 *   
 * 
 * Change History:
 * 
 * 
**************************************************************************/



using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dos.ORM
{
    public class WhereClipBuilder<T> : WhereClipBuilder
        where T : Entity
    {
        /// <summary>
        /// AND
        /// </summary>
        public void And(Expression<Func<T, bool>> lambdaWhere)
        {
            var tempWhere = ExpToWhereClip<T>.ToWhereClip(lambdaWhere);
            And(tempWhere);
        }
        /// <summary>
        /// Or
        /// </summary>
        public void Or(Expression<Func<T, bool>> lambdaWhere)
        {
            var tempWhere = ExpToWhereClip<T>.ToWhereClip(lambdaWhere);
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
