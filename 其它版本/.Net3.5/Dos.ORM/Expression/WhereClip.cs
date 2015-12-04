/*************************************************************************
 * 
 * Hxj.Data
 * 
 * 2010-2-10
 * 
 * steven hu   
 *  
 * Support: http://www.cnblogs.com/huxj
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
    [Obsolete("建议使用Where类替代WhereClip类。使用示例：var W = new Where<T>(); W.And(d=>d.ID == 1);")]
    public class WhereClip : Expression
    {
        /// <summary>
        /// All
        /// </summary>
        public readonly static WhereClip All = new WhereClip();
        #region 构造函数
        /// <summary>
        /// 
        /// </summary>
        public WhereClip() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="where"></param>
        public WhereClip(string where)
            : base(where)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customWhereString"></param>
        /// <param name="parameters"></param>
        public WhereClip(string customWhereString, params Parameter[] parameters)
            : base(customWhereString, parameters)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="oper"></param>
        public WhereClip(Field field, object value, QueryOperator oper)
            : base(field, value, oper)
        {

        }


        #endregion

        #region 属性



        /// <summary>
        /// 返回  where
        /// </summary>
        public string Where
        {
            get
            {
                return this.ToString();
            }
        }


        /// <summary>
        /// WhereString    
        /// <example>
        /// where 1=1
        /// </example>
        /// </summary>
        public string WhereString
        {
            get
            {
                if (string.IsNullOrEmpty(this.expressionString))
                    return string.Empty;

                return string.Concat(" WHERE ", this.expressionString);
            }
        }


        #endregion

        #region 方法

        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="whereString"></param>
        /// <returns></returns>
        public static implicit operator WhereClip(string whereString)
        {
            return new WhereClip(whereString);
        }


        /// <summary>
        /// 判断条件是否一样
        /// </summary>
        /// <param name="leftWhere"></param>
        /// <param name="rightWhere"></param>
        /// <returns></returns>
        public static bool Equals(WhereClip leftWhere, WhereClip rightWhere)
        {
            string leftWhereString = leftWhere.ToString();
            string rightWhereString = rightWhere.ToString();

            foreach (Parameter p in leftWhere.parameters)
            {
                leftWhereString.Replace(p.ParameterName, (p.ParameterValue == null) ? string.Empty : p.ParameterValue.ToString());
            }

            foreach (Parameter p in rightWhere.parameters)
            {
                rightWhereString.Replace(p.ParameterName, (p.ParameterValue == null) ? string.Empty : p.ParameterValue.ToString());
            }

            return (string.Compare(leftWhereString, rightWhereString, true) == 0);
        }




        /// <summary>
        /// 判断 WhereClip  是否为null
        /// </summary>
        /// <param name="whereClip"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(WhereClip whereClip)
        {
            if ((null == whereClip) || string.IsNullOrEmpty(whereClip.expressionString))
                return true;
            return false;
        }


        public override string ToString()
        {
            if (string.IsNullOrEmpty(expressionString))
                return string.Empty;

            return string.Concat("(", expressionString, ")");
        }


        public override bool Equals(object obj)
        {
            if (null == obj)
                return false;
            else if (obj is WhereClip)
            {
                return obj.ToString().Equals(this.ToString());
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// And
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public WhereClip And(WhereClip where)
        {
            if (WhereClip.IsNullOrEmpty(this) && WhereClip.IsNullOrEmpty(where))
                return All;

            if (WhereClip.IsNullOrEmpty(where))
                return this;
            if (WhereClip.IsNullOrEmpty(this))
                return where;



            WhereClip andwhere = new WhereClip(string.Concat(this.Where, " AND ", where.Where));
            andwhere.parameters.AddRange(this.Parameters);
            andwhere.parameters.AddRange(where.Parameters);


            return andwhere;
        }

        /// <summary>
        /// Or
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public WhereClip Or(WhereClip where)
        {
            if (WhereClip.IsNullOrEmpty(this) && WhereClip.IsNullOrEmpty(where))
                return All;

            if (WhereClip.IsNullOrEmpty(where))
                return this;
            if (WhereClip.IsNullOrEmpty(this))
                return where;

            WhereClip orwhere = new WhereClip(string.Concat(this.Where, " OR ", where.Where));
            orwhere.parameters.AddRange(this.Parameters);
            orwhere.parameters.AddRange(where.Parameters);


            return orwhere;
        }


        #region 重载操作符


        /// <summary>
        /// 
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator true(WhereClip right)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator false(WhereClip right)
        {
            return false;
        }



        /// <summary>
        /// And
        /// </summary>
        /// <param name="leftWhere"></param>
        /// <param name="rightWhere"></param>
        /// <returns></returns>
        public static WhereClip operator &(WhereClip leftWhere, WhereClip rightWhere)
        {
            if (WhereClip.IsNullOrEmpty(leftWhere))
                return rightWhere;

            return leftWhere.And(rightWhere);
        }

        /// <summary>
        /// Or
        /// </summary>
        /// <param name="leftWhere"></param>
        /// <param name="rightWhere"></param>
        /// <returns></returns>
        public static WhereClip operator |(WhereClip leftWhere, WhereClip rightWhere)
        {
            if (WhereClip.IsNullOrEmpty(leftWhere))
                return rightWhere;

            return leftWhere.Or(rightWhere);
        }

        /// <summary>
        /// not
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public static WhereClip operator !(WhereClip where)
        {
            if (IsNullOrEmpty(where))
            {
                return All;
            }
            return new WhereClip(string.Concat(" NOT ", where.expressionString), where.parameters.ToArray());
        }


        /// <summary>
        /// EXISTS
        /// </summary>
        /// <param name="fromSection"></param>
        /// <returns></returns>
        public static WhereClip Exists(FromSection fromSection)
        {
            return new WhereClip(string.Concat(" EXISTS (", fromSection.SqlString, ") "), fromSection.Parameters.ToArray());
        }

        #endregion



        #endregion



    }
}
