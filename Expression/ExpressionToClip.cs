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
* 修 改 人：ITdos
* 修改日期：
* 备注描述：
*******************************************************/
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Dos.ORM;
using System.Linq.Expressions;
using Dos.ORM.Common;

namespace Dos.ORM
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ExpressionToClip<T>
    {
        private static Evaluator evaluator = new Evaluator();
        private static CacheEvaluator cacheEvaluator = new CacheEvaluator();
        private static FastEvaluator fastEvaluator = new FastEvaluator();
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static WhereClip ToJoinWhere<TEntity>(Expression<Func<T, TEntity, bool>> e)
        {
            return ToWhereClipChild(e.Body, WhereType.JoinWhere);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static WhereClip ToWhereClip(Expression<Func<T, bool>> e)
        {
            return ToWhereClipChild(e.Body);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static WhereClip ToWhereClip<T2>(Expression<Func<T, T2, bool>> e)
        {
            return ToWhereClipChild(e.Body);
        }
        public static WhereClip ToWhereClip<T2, T3>(Expression<Func<T, T2, T3, bool>> e)
        {
            return ToWhereClipChild(e.Body);
        }
        public static WhereClip ToWhereClip<T2, T3, T4>(Expression<Func<T, T2, T3, T4, bool>> e)
        {
            return ToWhereClipChild(e.Body);
        }
        public static WhereClip ToWhereClip<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, bool>> e)
        {
            return ToWhereClipChild(e.Body);
        }
        public static WhereClip ToWhereClip<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> e)
        {
            return ToWhereClipChild(e.Body);
        }
        private static WhereClip ToWhereClipChild(System.Linq.Expressions.Expression e, WhereType wt = WhereType.Where)
        {
            if (e is BinaryExpression)
            {
                return ConvertBinary((BinaryExpression)e, wt);
            }
            if (e is MethodCallExpression)
            {
                return ConvertMethodCall((MethodCallExpression)e);
            }
            if (e is UnaryExpression)
            {
                return ConvertUnary((UnaryExpression)e);
            }
            if (IsBoolFieldOrProperty(e))
            {
                var key = ((MemberExpression)e).Member.Name;
                return new WhereClip();
            }
            if (e is ConstantExpression)
            {
                var key = ((ConstantExpression)e).Value;
                if (DataUtils.ConvertValue<bool>(key))
                {
                    return new WhereClip(" 1=1 ");
                }
                return new WhereClip(" 1=2 ");
            }
            throw new Exception("暂时不支持的Where条件Lambda表达式写法！请使用经典写法！");
        }

        private static bool IsBoolFieldOrProperty(System.Linq.Expressions.Expression e)
        {
            if (!(e is MemberExpression)) return false;
            var member = ((MemberExpression)e);
            if (member.Member.MemberType != MemberTypes.Field && member.Member.MemberType != MemberTypes.Property)
                return false;
            return member.Type == typeof(bool);
        }

        private static WhereClip ConvertUnary(UnaryExpression ue, WhereType wtype = WhereType.Where)
        {
            switch (ue.NodeType)
            {
                case ExpressionType.Not:
                    return !ToWhereClipChild(ue.Operand, wtype);
            }
            throw new Exception("暂时不支持的NodeType(" + ue.NodeType + ") lambda写法！请使用经典写法！");
        }

        private static WhereClip ConvertBinary(BinaryExpression be, WhereType wt = WhereType.Where)
        {
            switch (be.NodeType)
            {
                case ExpressionType.Equal:
                    return LeftAndRight(be, QueryOperator.Equal, wt);
                case ExpressionType.GreaterThan:
                    return LeftAndRight(be, QueryOperator.Greater, wt);
                case ExpressionType.GreaterThanOrEqual:
                    return LeftAndRight(be, QueryOperator.GreaterOrEqual, wt);
                case ExpressionType.LessThan:
                    return LeftAndRight(be, QueryOperator.Less, wt);
                case ExpressionType.LessThanOrEqual:
                    return LeftAndRight(be, QueryOperator.LessOrEqual, wt);
                case ExpressionType.NotEqual:
                    return LeftAndRight(be, QueryOperator.NotEqual, wt);
                case ExpressionType.AndAlso:
                    return ToWhereClipChild(be.Left, wt) && ToWhereClipChild(be.Right, wt);
                case ExpressionType.OrElse:
                    return ToWhereClipChild(be.Left, wt) || ToWhereClipChild(be.Right, wt);
                default:
                    throw new Exception("暂时不支持的Where条件(" + be.NodeType + ")Lambda表达式写法！请使用经典写法！");
            }
        }

        private static WhereClip ConvertMethodCall(MethodCallExpression mce)
        {
            switch (mce.Method.Name)
            {
                case "StartsWith":
                    return ConvertLikeCall(mce, "", "%");
                case "EndsWith":
                    return ConvertLikeCall(mce, "%", "");
                case "Contains":
                    return ConvertLikeCall(mce, "%", "%");
                case "Like":
                    return ConvertLikeCall(mce, "%", "%", true);
                case "Equals":
                    return ConvertEqualsCall(mce);
                case "In":
                    return ConvertInCall(mce);
                case "NotIn":
                    return ConvertInCall(mce, true);
                case "IsNull":
                    return ConvertNull(mce, true);
                case "IsNotNull":
                    return ConvertNull(mce);
                //case "Sum":
                //    return ConvertAs(e);
            }
            throw new Exception("暂时不支持的Lambda表达式方法: " + mce.Method.Name + "！请使用经典写法！");
        }

        private static WhereClip ConvertNull(MethodCallExpression mce, bool isNull = false)
        {
            ColumnFunction function;
            MemberExpression member;
            var key = GetMemberName(mce.Arguments[0], out function, out member);
            return isNull ? new Field(key, GetTableName(member.Expression.Type)).IsNull()
                : new Field(key, GetTableName(member.Expression.Type)).IsNotNull();
        }
        private static WhereClip ConvertEqualsCall(MethodCallExpression mce, bool isLike = false)
        {
            ColumnFunction function;
            MemberExpression member;
            var key = GetMemberName(mce.Object, out function, out member);
            var value = GetValue(mce.Arguments[0]);
            if (value != null)
            {
                return new WhereClip(new Field(key, GetTableName(member.Expression.Type)),
                    string.Concat(value), QueryOperator.Equal);
            }
            throw new Exception("'Like'仅支持一个参数，参数应为字符串且不允许为空");
        }
        private static WhereClip ConvertInCall(MethodCallExpression mce, bool notIn = false)
        {
            ColumnFunction function;
            MemberExpression member;
            var key = GetMemberName(mce.Arguments[0], out function, out member);
            var list = new List<object>();
            var ie = GetValue(mce.Arguments[1]);
            if (ie is IEnumerable)
            {
                list.AddRange(((IEnumerable)GetValue(mce.Arguments[1])).Cast<object>());
            }
            else
            {
                list.Add(ie);
            }
            return notIn ? new Field(key, GetTableName(member.Expression.Type)).SelectNotIn(list.ToArray())
                : new Field(key, GetTableName(member.Expression.Type)).SelectIn(list.ToArray());
        }

        private static WhereClip ConvertLikeCall(MethodCallExpression mce, string left, string right, bool isLike = false)
        {
            ColumnFunction function;
            MemberExpression member;
            var key = GetMemberName(isLike ? mce.Arguments[0] : mce.Object, out function, out member);
            if (isLike ? mce.Arguments.Count == 2 : mce.Arguments.Count == 1)
            {
                var value = GetValue(isLike ? mce.Arguments[1] : mce.Arguments[0]);
                if (value != null && value is string)
                {
                    return new WhereClip(new Field(key, GetTableName(member.Expression.Type)),
                        string.Concat(left, value, right), QueryOperator.Like);
                }
            }
            throw new Exception("'Like'仅支持一个参数，参数应为字符串且不允许为空");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="function"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
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
                return obj.Member.Name;
            }
            if (expr is MethodCallExpression)
            {
                var e = (MethodCallExpression)expr;
                if (e.Method.Name == "ToLower" && e.Object is MemberExpression)
                {
                    function = ColumnFunction.ToLower;
                    obj = (MemberExpression)e.Object;
                    return obj.Member.Name;
                }
                if (e.Method.Name == "ToUpper" && e.Object is MemberExpression)
                {
                    function = ColumnFunction.ToUpper;
                    obj = (MemberExpression)e.Object;
                    return obj.Member.Name;
                }
                throw new Exception("暂时不支持的Lambda表达式写法！请使用经典写法！");
            }
            throw new Exception("暂时不支持的Lambda表达式写法！请使用经典写法！");
        }

        private static WhereClip LeftAndRight(BinaryExpression be, QueryOperator co, WhereType wtype = WhereType.Where)
        {
            ColumnFunction leftFunction;
            ColumnFunction rightFunction;
            MemberExpression leftMe = null;
            MemberExpression rightMe;
            System.Linq.Expressions.Expression expLeft = be.Left;
            System.Linq.Expressions.Expression expRight = be.Right;
            if (be.Left.NodeType == ExpressionType.Convert)
            {
                expLeft = ((UnaryExpression)be.Left).Operand;
            }
            if (be.Right.NodeType == ExpressionType.Convert)
            {
                expRight = ((UnaryExpression)be.Right).Operand;
            }
            var isAgain = false;
        Again:
            if (expLeft.NodeType == ExpressionType.Constant
                || (expLeft.NodeType == ExpressionType.MemberAccess && ((MemberExpression)expLeft).Expression == null) || isAgain)
            {
                if (expRight.NodeType == ExpressionType.Constant ||
                    (expRight.NodeType == ExpressionType.MemberAccess && ((MemberExpression)expRight).Expression == null))
                {
                    return DataUtils.ConvertValue<bool>(fastEvaluator.Eval(be))
                        ? new WhereClip(" 1=2 ")
                        : new WhereClip(" 1=1 ");
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
                                return new WhereClip(new Field(keyLeft, GetTableName(left.Expression.Type)), GetValue(expRight), co);
                            }
                            else
                            {
                                return new WhereClip(new Field(keyRightName, GetTableName(rightMe.Expression.Type)), new Field(keyLeft, GetTableName(left.Expression.Type)), co);
                            }
                        }
                    }
                    object value = GetValue(expLeft);
                    if (keyRightName.Contains("$"))
                    {
                        if (DataUtils.ConvertValue<bool>(fastEvaluator.Eval(be)))
                        {
                            return new WhereClip(" 1=2 ");
                        }
                        return new WhereClip(" 1=1 ");
                    }
                    if (value != null)
                        return new WhereClip(new Field(keyRightName, GetTableName(rightMe.Expression.Type)), value, co);
                    switch (co)
                    {
                        case QueryOperator.Equal:
                            return new Field(keyRightName, GetTableName(rightMe.Expression.Type)).IsNull();
                        case QueryOperator.NotEqual:
                            return new Field(keyRightName, GetTableName(rightMe.Expression.Type)).IsNotNull();
                    }
                    throw new Exception("null值只支持等于或不等于！出错比较符：" + co.ToString());
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
                        return new WhereClip(
                            new Field(key, GetTableName(leftMe.Expression.Type)),
                            new Field(keyRight, GetTableName(right.Expression.Type))
                            , co);
                    }
                }
                object value = GetValue(expRight);
                if (value == null)
                {
                    if (co == QueryOperator.Equal)
                    {
                        return new Field(key, GetTableName(leftMe.Expression.Type)).IsNull();
                    }
                    if (co == QueryOperator.NotEqual)
                    {
                        return new Field(key, GetTableName(leftMe.Expression.Type)).IsNotNull();
                    }
                    throw new Exception("null值只支持等于或不等于！");
                }
                return new WhereClip(new Field(key, GetTableName(leftMe.Expression.Type)), value, co);
            }
        }

        private static object GetValue(System.Linq.Expressions.Expression right)
        {
            return fastEvaluator.Eval(right);
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
                return new GroupByClip(new Field(e.Member.Name, GetTableName(e.Expression.Type)));
            }
            if (exprBody is NewExpression)
            {
                var exNew = (NewExpression)exprBody;
                var type = exNew.Constructor.DeclaringType;
                var list = new List<string>(exNew.Arguments.Count);
                return exNew.Arguments.Cast<MemberExpression>().Aggregate(GroupByClip.None, (current, member)
                    => current && new Field(member.Member.Name, GetTableName(member.Expression.Type)).GroupBy);
            }
            if (exprBody is UnaryExpression)
            {
                var exNew = (UnaryExpression)exprBody;
                return ToGroupByClipChild(exNew.Operand);
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
                    gb = gb && new Field(e.Member.Name, GetTableName(e.Expression.Type)).Desc;
                }
                else
                {
                    gb = gb && new Field(e.Member.Name, GetTableName(e.Expression.Type)).Asc;
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
                        gb = gb && new Field(member.Member.Name, GetTableName(member.Expression.Type)).Desc;
                    }
                    else
                    {
                        gb = gb && new Field(member.Member.Name, GetTableName(member.Expression.Type)).Asc;
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
        public static Field[] ToSelect<T2, T3, T4, T5>(Expression<Func<T, T2, T3, T4, T5, object>> expr)
        {
            return ToSelectChild(expr.Body);
        }
        public static Field[] ToSelect<T2, T3, T4, T5, T6>(Expression<Func<T, T2, T3, T4, T5, T6, object>> expr)
        {
            return ToSelectChild(expr.Body);
        }
        public static Field[] ToSelect(Expression<Func<T, bool>> expr)
        {
            return ToSelectChild(expr.Body);
        }
        private static Field[] ToSelectChild(System.Linq.Expressions.Expression exprBody)
        {
            if (exprBody is MemberExpression)
            {
                var e = (MemberExpression)exprBody;
                return new[] { new Field(e.Member.Name, GetTableName(e.Expression.Type)) };
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
                foreach (var item in exNew.Arguments)
                {
                    var aliasName = exNew.Members[i].Name;
                    if (item is MemberExpression)
                    {
                        var member = (MemberExpression)item;
                        if (member.Member.Name != aliasName)
                        {
                            f[i] = new Field(member.Member.Name, GetTableName(member.Expression.Type), null, null, "", aliasName);
                        }
                        else
                        {
                            f[i] = new Field(member.Member.Name, GetTableName(member.Expression.Type));
                        }
                    }
                    else if (item is MethodCallExpression)
                    {
                        var member = (MethodCallExpression)item;
                        f[i] = ConvertFun(member, aliasName)[0];
                    }
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
        private static Field[] ConvertFun(MethodCallExpression e, string aliasName = null)
        {
            ColumnFunction function;
            MemberExpression member;
            var key = GetMemberName(e.Arguments[0], out function, out member);
            Field f;
            f = string.IsNullOrWhiteSpace(aliasName)
                ? new Field(key, GetTableName(member.Expression.Type))
                : new Field(key, GetTableName(member.Expression.Type), null, null, "", aliasName);
            switch (e.Method.Name)
            {
                case "Sum":
                    return new[] { f.Sum() };
                case "Avg":
                    return new[] { f.Avg() };
                case "Len":
                    return new[] { f.Len() };
                case "Count":
                    return new[] { f.Count() };
                default:
                    throw new Exception("暂时不支持的Lambda表达式写法(" + e.Method.Name + ")！请使用经典写法！");
            }
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
                    return new[] { new Field(key, GetTableName(member.Expression.Type)) { AliasName = value.ToString() } };
                }
            }
            throw new Exception("'As'仅支持一个参数，参数应为字符串且不允许为空");
        }

        private static string GetTableName(Type type)
        {
            var tbl = type.GetCustomAttribute<Table>(false);
            return tbl != null ? tbl.GetTableName() : type.Name;
        }
    }
}