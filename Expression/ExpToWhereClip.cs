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
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Dos.ORM;
using System.Linq.Expressions;
using Dos.ORM.Common;

namespace Dos.ORM
{
    public static class ExpToWhereClip<T>
    {
        public static WhereClip ToGroupByClip(Expression<Func<T, bool>> expr)
        {
            return ToClip(expr.Body);
        }
        public static WhereClip ToWhereClip(Expression<Func<T, bool>> expr)
        {
            return ToClip(expr.Body);
        }

        private static WhereClip ToClip(System.Linq.Expressions.Expression exprBody)
        {
            if (exprBody is BinaryExpression)
            {
                return ConvertBinary((BinaryExpression)exprBody);
            }
            if (exprBody is MethodCallExpression)
            {
                return ConvertMethodCall((MethodCallExpression)exprBody);
            }
            if(exprBody is UnaryExpression)
            {
                return ConvertUnary((UnaryExpression)exprBody);
            }
            if (IsBoolFieldOrProperty(exprBody))
            {
                var key = ((MemberExpression)exprBody).Member.Name;
                return new WhereClip();
            }
            throw new Exception("不支持的Where条件！");
        }

        private static bool IsBoolFieldOrProperty(System.Linq.Expressions.Expression expr)
        {
            if (expr is MemberExpression)
            {
                var member = ((MemberExpression)expr);
                if(member.Member.MemberType == MemberTypes.Field || member.Member.MemberType == MemberTypes.Property)
                {
                    if(member.Type == typeof(bool))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static WhereClip ConvertUnary(UnaryExpression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.Not:
                    return !ToClip(expr.Operand);
            }
            throw new Exception("不支持的NodeType");
        }

        private static WhereClip ConvertBinary(BinaryExpression e)
        {
            switch (e.NodeType)
            {
                case ExpressionType.Equal:
                    return GetClause(e, QueryOperator.Equal);
                case ExpressionType.GreaterThan:
                    return GetClause(e, QueryOperator.Greater);
                case ExpressionType.GreaterThanOrEqual:
                    return GetClause(e, QueryOperator.GreaterOrEqual);
                case ExpressionType.LessThan:
                    return GetClause(e, QueryOperator.Less);
                case ExpressionType.LessThanOrEqual:
                    return GetClause(e, QueryOperator.LessOrEqual);
                case ExpressionType.NotEqual:
                    return GetClause(e, QueryOperator.NotEqual);
                case ExpressionType.AndAlso:
                    return ToClip(e.Left) && ToClip(e.Right);
                case ExpressionType.OrElse:
                    return ToClip(e.Left) || ToClip(e.Right);
                default:
                    throw new Exception("不支持的Where写法！");
            }
        }

        private static WhereClip ConvertMethodCall(MethodCallExpression e)
        {
            switch (e.Method.Name)
            {
                case "StartsWith":
                    return ConvertLikeCall(e, "", "%");
                case "EndsWith":
                    return ConvertLikeCall(e, "%", "");
                case "Contains":
                    return ConvertLikeCall(e, "%", "%");
                case "In":
                case "InStatement":
                    return ConvertInCall(e);
                case "NotIn":
                case "NotInStatement":
                    return ConvertInCall(e, notIn: true);
                case "IsNull":
                    return ConvertNull(e, true);
                case "IsNotNull":
                    return ConvertNull(e, false);
            }
            throw new Exception("不支持的方法: " + e.Method.Name);
        }

        private static WhereClip ConvertNull(MethodCallExpression e, bool isNull)
        {
            ColumnFunction function;
            MemberExpression member;
            string key = GetMemberName(e.Arguments[0], out function, out member);
                    return  new WhereClip();
        }

        private static WhereClip ConvertInCall(MethodCallExpression e, bool notIn = false)
        {
            ColumnFunction function;
            MemberExpression member;
            string key = GetMemberName(e.Arguments[0], out function, out member);
            var list = new List<object>();
            var ie = GetRightValue(e.Arguments[1]);
            if (ie is IEnumerable)
            {
                foreach (var obj in (IEnumerable)GetRightValue(e.Arguments[1]))
                {
                    list.Add(obj);
                }
            }
            else
            {
                list.Add(ie);
            }
            return new WhereClip();
        }

        private static WhereClip ConvertLikeCall(MethodCallExpression e, string left, string right)
        {
            ColumnFunction function;
            MemberExpression member;
            string key = GetMemberName(e.Object, out function, out member);
            if(e.Arguments.Count == 1)
            {
                object value = GetRightValue(e.Arguments[0]);
                if (value != null && value is string)
                {
                    return  new WhereClip();
                }
            }
            throw new Exception("'Like'仅支持一个参数，参数应为字符串且不允许为空");
        }

        private static string GetColumnName(MemberExpression expr)
        {
            string mn = expr.Member.Name;
            if (expr.Expression is MemberExpression)
            {
                var m = (MemberExpression)expr.Expression;
                if (mn == "Id")
                {
                    mn = m.Member.Name;
                }
                else
                {
                    mn = m.Member.Name + "$" + mn;
                }
            }
            return mn;
        }

        public static string GetMemberName(System.Linq.Expressions.Expression expr, out ColumnFunction function, out MemberExpression obj)
        {
            if (expr.NodeType == ExpressionType.Convert)
            {
                expr = ((UnaryExpression)expr).Operand;
            }
            if (expr is MemberExpression)
            {
                function = ColumnFunction.None;
                obj = (MemberExpression)expr;
                return GetColumnName(obj);
            }
            if(expr is MethodCallExpression)
            {
                var e = (MethodCallExpression) expr;
                if(e.Method.Name == "ToLower" && e.Object is MemberExpression)
                {
                    function = ColumnFunction.ToLower;
                    obj = (MemberExpression)e.Object;
                    return GetColumnName(obj);
                }
                if (e.Method.Name == "ToUpper" && e.Object is MemberExpression)
                {
                    function = ColumnFunction.ToUpper;
                    obj = (MemberExpression)e.Object;
                    return GetColumnName(obj);
                }
            }
            throw new Exception("表达式必须为'Column op const' 或者 'Column op Column'");
        }

        private static WhereClip GetClause(BinaryExpression e, QueryOperator co)
        {
            ColumnFunction function;
            MemberExpression left;
            var key = GetMemberName(e.Left, out function, out left);
            string pn = left.Expression.ToString();
            if (e.Right.NodeType == ExpressionType.MemberAccess)
            {
                var right = (MemberExpression)e.Right;
                if (right.Expression != null && right.Expression.ToString() == pn)
                {
                    string key2 = right.Member.Name;
                    return new WhereClip();
                }
            }
            object value = GetRightValue(e.Right);
            if (value == null)
            {
                if (co == QueryOperator.Equal)
                {
                    return new Field(key, left.Expression.Type.Name).IsNull();
                }
                if (co == QueryOperator.NotEqual)
                {
                    return new Field(key, left.Expression.Type.Name).IsNotNull();
                }
                throw new Exception("null值只支持等于或不等于！");
            }
            return new WhereClip(new Field(key, left.Expression.Type.Name), value, co);
        }

        private static object GetRightValue(System.Linq.Expressions.Expression right)
        {
            object value 
                = right.NodeType == ExpressionType.Constant 
                      ? ((ConstantExpression)right).Value
                      : System.Linq.Expressions.Expression.Lambda(right).Compile().DynamicInvoke();
            return value;
        }
    }
}