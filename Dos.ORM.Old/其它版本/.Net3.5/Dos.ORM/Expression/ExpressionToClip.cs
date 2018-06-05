#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：
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
using System.Runtime.CompilerServices;
using Dos.ORM;
using System.Linq.Expressions;
using Dos.ORM.Common;

namespace Dos.ORM
{
    public static class ExpressionToClip<T>
    {
        private static Evaluator evaluator = new Evaluator();
        private static CacheEvaluator cacheEvaluator = new CacheEvaluator();
        private static FastEvaluator fastEvaluator = new FastEvaluator();
        /// <summary>
        /// 
        /// </summary>
        public static WhereClip ToJoinWhere<TEntity>(Expression<Func<T, TEntity, bool>> expr)
        {
            return ToWhereClipChild(expr.Body, WhereType.JoinWhere);
        }
        /// <summary>
        /// 
        /// </summary>
        public static WhereClip ToWhereClip(Expression<Func<T, bool>> expr)
        {
            return ToWhereClipChild(expr.Body);
        }
        public static WhereClip ToWhereClip<T2>(Expression<Func<T, T2, bool>> expr)
        {
            return ToWhereClipChild(expr.Body);
        }
        public static WhereClip ToWhereClip<T2, T3>(Expression<Func<T, T2, T3, bool>> expr)
        {
            return ToWhereClipChild(expr.Body);
        }
        public static WhereClip ToWhereClip<T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> expr)
        {
            return ToWhereClipChild(expr.Body);
        }
        //public static WhereClip ToWhereClip<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> expr)
        //{
        //    return ToWhereClipChild(expr.Body);
        //}
        //public static WhereClip ToWhereClip<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expr)
        //{
        //    return ToWhereClipChild(expr.Body);
        //}
        private static WhereClip ToWhereClipChild(System.Linq.Expressions.Expression exprBody, WhereType wtype = WhereType.Where)
        {
            if (exprBody is BinaryExpression)
            {
                return ConvertBinary((BinaryExpression)exprBody, wtype);
            }
            if (exprBody is MethodCallExpression)
            {
                return ConvertMethodCall((MethodCallExpression)exprBody);
            }
            if (exprBody is UnaryExpression)
            {
                return ConvertUnary((UnaryExpression)exprBody);
            }
            if (IsBoolFieldOrProperty(exprBody))
            {
                var key = ((MemberExpression)exprBody).Member.Name;
                return new WhereClip();
            }
            if (exprBody is ConstantExpression)
            {
                var key = ((ConstantExpression)exprBody).Value;
                if (DataUtils.ConvertValue<bool>(key))
                {
                    return new WhereClip(" 1=1 ");
                }
                return new WhereClip(" 1=2 ");
            }
            throw new Exception("暂时不支持的Where条件Lambda表达式写法！请使用经典写法！");
        }

        private static bool IsBoolFieldOrProperty(System.Linq.Expressions.Expression expr)
        {
            if (expr is MemberExpression)
            {
                var member = ((MemberExpression)expr);
                if (member.Member.MemberType == MemberTypes.Field || member.Member.MemberType == MemberTypes.Property)
                {
                    if (member.Type == typeof(bool))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static WhereClip ConvertUnary(UnaryExpression expr, WhereType wtype = WhereType.Where)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.Not:
                    return !ToWhereClipChild(expr.Operand, wtype);
            }
            throw new Exception("暂时不支持的NodeType(" + expr.NodeType + ") lambda写法！请使用经典写法！");
        }

        private static WhereClip ConvertBinary(BinaryExpression e, WhereType wtype = WhereType.Where)
        {
            switch (e.NodeType)
            {
                case ExpressionType.Equal:
                    return LeftAndRight(e, QueryOperator.Equal, wtype);
                case ExpressionType.GreaterThan:
                    return LeftAndRight(e, QueryOperator.Greater, wtype);
                case ExpressionType.GreaterThanOrEqual:
                    return LeftAndRight(e, QueryOperator.GreaterOrEqual, wtype);
                case ExpressionType.LessThan:
                    return LeftAndRight(e, QueryOperator.Less, wtype);
                case ExpressionType.LessThanOrEqual:
                    return LeftAndRight(e, QueryOperator.LessOrEqual, wtype);
                case ExpressionType.NotEqual:
                    return LeftAndRight(e, QueryOperator.NotEqual, wtype);
                case ExpressionType.AndAlso:
                    return ToWhereClipChild(e.Left, wtype) && ToWhereClipChild(e.Right, wtype);
                case ExpressionType.OrElse:
                    return ToWhereClipChild(e.Left, wtype) || ToWhereClipChild(e.Right, wtype);
                default:
                    throw new Exception("暂时不支持的Where条件(" + e.NodeType + ")Lambda表达式写法！请使用经典写法！");
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
                case "Like":
                    return ConvertLikeCall(e, "%", "%", true);
                case "In":
                    return ConvertInCall(e);
                case "NotIn":
                    return ConvertInCall(e, true);
                case "IsNull":
                    return ConvertNull(e, true);
                case "IsNotNull":
                    return ConvertNull(e);
                //case "Sum":
                //    return ConvertAs(e);
            }
            throw new Exception("暂时不支持的Lambda表达式方法: " + e.Method.Name + "！请使用经典写法！");
        }

        private static WhereClip ConvertNull(MethodCallExpression e, bool isNull = false)
        {
            ColumnFunction function;
            MemberExpression member;
            string key = GetMemberName(e.Arguments[0], out function, out member);
            if (isNull)
            {
                return new Field(key, member.Expression.Type.Name).IsNull();
            }
            return new Field(key, member.Expression.Type.Name).IsNotNull();
        }

        private static WhereClip ConvertInCall(MethodCallExpression e, bool notIn = false)
        {
            ColumnFunction function;
            MemberExpression member;
            string key = GetMemberName(e.Arguments[0], out function, out member);
            var list = new List<object>();
            var ie = GetValue(e.Arguments[1]);
            if (ie is IEnumerable)
            {
                foreach (var obj in (IEnumerable)GetValue(e.Arguments[1]))
                {
                    list.Add(obj);
                }
            }
            else
            {
                list.Add(ie);
            }
            if (notIn)
            {
                return new Field(key, member.Expression.Type.Name).SelectNotIn(list.ToArray());
            }
            return new Field(key, member.Expression.Type.Name).SelectIn(list.ToArray());
        }

        private static WhereClip ConvertLikeCall(MethodCallExpression e, string left, string right, bool isLike = false)
        {
            ColumnFunction function;
            MemberExpression member;
            string key = GetMemberName(isLike ? e.Arguments[0] : e.Object, out function, out member);
            if (isLike ? e.Arguments.Count == 2 : e.Arguments.Count == 1)
            {
                object value = GetValue(isLike ? e.Arguments[1] : e.Arguments[0]);
                if (value != null && value is string)
                {
                    return new WhereClip(new Field(key, member.Expression.Type.Name),
                        string.Concat(left, value, right), QueryOperator.Like);
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
            if (expr is MethodCallExpression)
            {
                var e = (MethodCallExpression)expr;
                if (e.Method.Name == "ToLower" && e.Object is MemberExpression)
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
                //if (e.Method.Name == "Sum")
                //{
                //    return  e.Arguments.
                //}
                throw new Exception("暂时不支持的Lambda表达式写法！请使用经典写法！");
            }
            throw new Exception("暂时不支持的Lambda表达式写法！请使用经典写法！");
        }

        private static WhereClip LeftAndRight(BinaryExpression e, QueryOperator co, WhereType wtype = WhereType.Where)
        {
            ColumnFunction leftFunction;
            ColumnFunction rightFunction;
            MemberExpression leftMe = null;
            MemberExpression rightMe;
            System.Linq.Expressions.Expression expLeft = e.Left;
            System.Linq.Expressions.Expression expRight = e.Right;
            if (e.Left.NodeType == ExpressionType.Convert)
            {
                expLeft = ((UnaryExpression)e.Left).Operand;
            }
            if (e.Right.NodeType == ExpressionType.Convert)
            {
                expRight = ((UnaryExpression)e.Right).Operand;
            }

            //DebugInfoGenerator aa +=        ;
            //System.Linq.Expressions.Expression.Lambda(expRight).Compile(debugInfoGenerator);

            var isAgain = false;
        Again:
            if (expLeft.NodeType == ExpressionType.Constant
                || (expLeft.NodeType == ExpressionType.MemberAccess && ((MemberExpression)expLeft).Expression == null) || isAgain)
            {
                if (expRight.NodeType == ExpressionType.Constant ||
                    (expRight.NodeType == ExpressionType.MemberAccess && ((MemberExpression)expRight).Expression == null))
                {
                    //var leftValue = GetValue(expLeft);
                    //var rightValue = GetValue(expRight);
                    //if (leftValue.Equals(rightValue))
                    //{
                    //    return new WhereClip(" 1=1 ");
                    //}
                    if (DataUtils.ConvertValue<bool>(fastEvaluator.Eval(e)))
                    {
                        return new WhereClip(" 1=2 ");
                    }
                    return new WhereClip(" 1=1 ");
                }
                else
                {
                    var keyRightName = GetMemberName(expRight, out rightFunction, out rightMe);
                    
                    if (expLeft.NodeType == ExpressionType.MemberAccess)
                    {
                        var left = (MemberExpression)expLeft;
                        if (left.Expression != null && (wtype == WhereType.JoinWhere || left.Expression.ToString() == rightMe.Expression.ToString()))
                        {
                            ColumnFunction functionLeft;
                            var keyLeft = GetMemberName(expLeft, out functionLeft, out left);
                            if (keyRightName.Contains("$"))
                            {
                                return new WhereClip(new Field(keyLeft, left.Expression.Type.Name), GetValue(expRight), co);
                            }
                            else
                            {
                                return new WhereClip(new Field(keyRightName, rightMe.Expression.Type.Name), new Field(keyLeft, left.Expression.Type.Name), co);
                            }
                        }
                    }
                    object value = GetValue(expLeft);
                    if (keyRightName.Contains("$"))
                    {
                        if (DataUtils.ConvertValue<bool>(fastEvaluator.Eval(e)))
                        {
                            return new WhereClip(" 1=2 ");
                        }
                        return new WhereClip(" 1=1 ");
                    }
                    if (value == null)
                    {
                        if (co == QueryOperator.Equal)
                        {
                            return new Field(keyRightName, rightMe.Expression.Type.Name).IsNull();
                        }
                        if (co == QueryOperator.NotEqual)
                        {
                            return new Field(keyRightName, rightMe.Expression.Type.Name).IsNotNull();
                        }
                        throw new Exception("null值只支持等于或不等于！");
                    }
                    return new WhereClip(new Field(keyRightName, rightMe.Expression.Type.Name), value, co);
                }
            }
            else
            {
                var key = "";
                try
                {
                    key = GetMemberName(expLeft, out leftFunction, out leftMe);
                    if (key.Contains("$"))
                    {
                        isAgain = true;
                        goto Again;
                    }
                }
                catch (Exception)
                {
                    isAgain = true;
                    goto Again;
                }
                if (expRight.NodeType == ExpressionType.MemberAccess)
                {
                    var right = (MemberExpression)expRight;
                    if (right.Expression != null && (wtype == WhereType.JoinWhere || right.Expression == leftMe.Expression))
                    {
                        ColumnFunction functionRight;
                        var keyRight = GetMemberName(expRight, out functionRight, out right);
                        return new WhereClip(new Field(key, leftMe.Expression.Type.Name), new Field(keyRight, right.Expression.Type.Name), co);
                    }
                }
                object value = GetValue(expRight);
                if (value == null)
                {
                    if (co == QueryOperator.Equal)
                    {
                        return new Field(key, leftMe.Expression.Type.Name).IsNull();
                    }
                    if (co == QueryOperator.NotEqual)
                    {
                        return new Field(key, leftMe.Expression.Type.Name).IsNotNull();
                    }
                    throw new Exception("null值只支持等于或不等于！");
                }
                return new WhereClip(new Field(key, leftMe.Expression.Type.Name), value, co);
            }
        }

        private static object GetValue(System.Linq.Expressions.Expression right)
        {
            return fastEvaluator.Eval(right);
            //object value
            //    = right.NodeType == ExpressionType.Constant
            //          ? ((ConstantExpression)right).Value
            //          : System.Linq.Expressions.Expression.Lambda(right).Compile().DynamicInvoke();
            //return value;
        }

        public static GroupByClip ToGroupByClip(Expression<Func<T, object>> expr)
        {
            return ToGroupByClipChild(expr.Body);
        }
        private static GroupByClip ToGroupByClipChild(System.Linq.Expressions.Expression exprBody)
        {
            if (exprBody is MemberExpression)
            {
                var e = (MemberExpression)exprBody;
                return new GroupByClip(new Field(e.Member.Name, e.Expression.Type.Name));
            }
            if (exprBody is NewExpression)
            {
                var exNew = (NewExpression)exprBody;
                var type = exNew.Constructor.DeclaringType;
                var list = new List<string>(exNew.Arguments.Count);
                GroupByClip gb = GroupByClip.None;
                foreach (MemberExpression member in exNew.Arguments)
                {
                    gb = gb && new Field(member.Member.Name, member.Expression.Type.Name).GroupBy;
                }
                return gb;
            }
            if (exprBody is UnaryExpression)
            {
                var exNew = (UnaryExpression)exprBody;
                return ToGroupByClipChild(exNew.Operand);
                //var type = exNew.Constructor.DeclaringType;
                //var list = new List<string>(exNew.Arguments.Count);
                //GroupByClip gb = GroupByClip.None;
                //foreach (MemberExpression member in exNew.Arguments)
                //{
                //    gb = gb && new Field(member.Member.Name, member.Expression.Type.Name).GroupBy;
                //}
                //return gb;
            }
            
            throw new Exception("暂时不支持的Group by lambda写法！请使用经典写法！");
        }

        public static OrderByClip ToOrderByClip(Expression<Func<T, object>> expr)
        {
            return ToOrderByClipChild(expr.Body, OrderByOperater.ASC);
        }
        public static OrderByClip ToOrderByDescendingClip(Expression<Func<T, object>> expr)
        {
            return ToOrderByClipChild(expr.Body, OrderByOperater.DESC);
        }
        private static OrderByClip ToOrderByClipChild(System.Linq.Expressions.Expression exprBody, OrderByOperater orderBy)
        {
            if (exprBody is MemberExpression)
            {
                var e = (MemberExpression)exprBody;
                OrderByClip gb = OrderByClip.None;
                if (orderBy == OrderByOperater.DESC)
                {
                    gb = gb && new Field(e.Member.Name, e.Expression.Type.Name).Desc;
                }
                else
                {
                    gb = gb && new Field(e.Member.Name, e.Expression.Type.Name).Asc;
                }
                return gb;
            }
            if (exprBody is NewExpression)
            {
                var exNew = (NewExpression)exprBody;
                var type = exNew.Constructor.DeclaringType;
                var list = new List<string>(exNew.Arguments.Count);
                OrderByClip gb = OrderByClip.None;
                foreach (MemberExpression member in exNew.Arguments)
                {
                    if (orderBy == OrderByOperater.DESC)
                    {
                        gb = gb && new Field(member.Member.Name, member.Expression.Type.Name).Desc;
                    }
                    else
                    {
                        gb = gb && new Field(member.Member.Name, member.Expression.Type.Name).Asc;
                    }
                }
                return gb;
            }
            if (exprBody is UnaryExpression)
            {
                var ueEx = (UnaryExpression)exprBody;
                return ToOrderByClipChild(ueEx.Operand, orderBy);
            }
            throw new Exception("暂时不支持的Order by lambda写法！请使用经典写法！");
        }

        public static Field[] ToSelect(Expression<Func<T, object>> expr)
        {
            return ToSelectChild(expr.Body);
        }
        public static Field[] ToSelect<T2>(Expression<Func<T, T2, object>> expr)
        {
            return ToSelectChild(expr.Body);
        }
        public static Field[] ToSelect<T2, T3>(Expression<Func<T, T2, T3, object>> expr)
        {
            return ToSelectChild(expr.Body);
        }
        public static Field[] ToSelect<T2, T3, T4>(Expression<Func<T, T2, T3, T4, object>> expr)
        {
            return ToSelectChild(expr.Body);
        }
        //public static Field[] ToSelect<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object>> expr)
        //{
        //    return ToSelectChild(expr.Body);
        //}
        //public static Field[] ToSelect<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object>> expr)
        //{
        //    return ToSelectChild(expr.Body);
        //}
        public static Field[] ToSelect(Expression<Func<T, bool>> expr)
        {
            return ToSelectChild(expr.Body);
        }
        private static Field[] ToSelectChild(System.Linq.Expressions.Expression exprBody)
        {
            if (exprBody is MemberExpression)
            {
                var e = (MemberExpression)exprBody;
                return new[] { new Field(e.Member.Name, e.Expression.Type.Name) };
            }
            if (exprBody is MethodCallExpression)
            {
                var e = (MethodCallExpression)exprBody;
                switch (e.Method.Name)
                {
                    case "As":
                        return ConvertAs(e);
                    default:
                        return ConvertFun(e);
                }
                throw new Exception("暂时不支持的Select lambda写法！请使用经典写法！");
            }
            if (exprBody is NewExpression)
            {
                var exNew = (NewExpression)exprBody;
                var type = exNew.Constructor.DeclaringType;
                var list = new List<string>(exNew.Arguments.Count);
                var f = new Field[exNew.Arguments.Count];
                var i = 0;
                foreach (MemberExpression member in exNew.Arguments)
                {
                    f[i] = new Field(member.Member.Name, member.Expression.Type.Name);
                    i++;
                }
                return f;
            }
            if (exprBody is UnaryExpression)
            {
                var expr = (UnaryExpression)exprBody;
                switch (expr.NodeType)
                {
                    case ExpressionType.Convert:
                        return ToSelectChild(expr.Operand);
                }
            }
            throw new Exception("暂时不支持的Select lambda写法！请使用经典写法！");
        }
        private static Field[] ConvertFun(MethodCallExpression e)
        {
            ColumnFunction function;
            MemberExpression member;
            string key = GetMemberName(e.Arguments[0], out function, out member);
            switch (e.Method.Name)
            {
                case "Sum":
                    return new[] { new Field(key, member.Expression.Type.Name).Sum() };
                case "Avg":
                    return new[] { new Field(key, member.Expression.Type.Name).Avg() };
                case "Len":
                    return new[] { new Field(key, member.Expression.Type.Name).Len() };
            }
            throw new Exception("暂时不支持的Lambda表达式写法(" + e.Method.Name + ")！请使用经典写法！");
        }
        private static Field[] ConvertAs(MethodCallExpression e)
        {
            ColumnFunction function;
            MemberExpression member;
            string key = GetMemberName(e.Arguments[0], out function, out member);
            if (e.Arguments.Count == 2)
            {
                object value = GetValue(e.Arguments[1]);
                if (value != null && value is string)
                {
                    return new[] { new Field(key, member.Expression.Type.Name) { AliasName = value.ToString() } };
                }
            }
            throw new Exception("'As'仅支持一个参数，参数应为字符串且不允许为空");
        }
    }
}