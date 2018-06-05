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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;
using System.Collections.ObjectModel;

namespace Dos.ORM
{
    public class ExpressionComparer : IComparer<System.Linq.Expressions.Expression>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="result"></param>
        /// <returns>can stop comparing or not</returns>
        protected bool CompareNull<T>(T x, T y, out int result) where T : class
        {
            if (x == null && y == null)
            {
                result = 0;
                return true;
            }

            if (x == null || y == null)
            {
                result = x == null ? -1 : 1;
                return true;
            }

            result = 0;
            return false;
        }

        protected virtual int CompareType(Type x, Type y)
        {
            if (x == y) return 0;

            int result;
            if (this.CompareNull(x, y, out result)) return result;

            result = x.GetHashCode() - y.GetHashCode();
            if (result != 0) return result;

            result = x.Name.CompareTo(y.Name);
            if (result != 0) return result;

            return x.AssemblyQualifiedName.CompareTo(y.AssemblyQualifiedName);
        }

        protected virtual int CompareMemberInfo(MemberInfo x, MemberInfo y)
        {
            if (x == y) return 0;

            int result;
            if (this.CompareNull(x, y, out result)) return result;

            result = x.GetHashCode() - y.GetHashCode();
            if (result != 0) return result;

            result = x.MemberType - y.MemberType;
            if (result != 0) return result;

            result = x.Name.CompareTo(y.Name);
            if (result != 0) return result;

            result = CompareType(x.DeclaringType, y.DeclaringType);
            if (result != 0) return result;

            return x.ToString().CompareTo(y.ToString());
        }

        public virtual int Compare(System.Linq.Expressions.Expression x, System.Linq.Expressions.Expression y)
        {
            int result;
            if (this.CompareNull(x, y, out result)) return result;

            result = this.CompareType(x.GetType(), y.GetType());
            if (result != 0) return result;
                
            result = x.NodeType - y.NodeType;
            if (result != 0) return result;

            result = this.CompareType(x.Type, y.Type);
            if (result != 0) return result;

            switch (x.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return this.CompareUnary((UnaryExpression)x, (UnaryExpression)y);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return this.CompareBinary((BinaryExpression)x, (BinaryExpression)y);
                case ExpressionType.TypeIs:
                    return this.CompareTypeIs((TypeBinaryExpression)x, (TypeBinaryExpression)y);
                case ExpressionType.Conditional:
                    return this.CompareConditional((ConditionalExpression)x, (ConditionalExpression)y);
                case ExpressionType.Constant:
                    return this.CompareConstant((ConstantExpression)x, (ConstantExpression)y);
                case ExpressionType.Parameter:
                    return this.CompareParameter((ParameterExpression)x, (ParameterExpression)y);
                case ExpressionType.MemberAccess:
                    return this.CompareMemberAccess((MemberExpression)x, (MemberExpression)y);
                case ExpressionType.Call:
                    return this.CompareMethodCall((MethodCallExpression)x, (MethodCallExpression)y);
                case ExpressionType.Lambda:
                    return this.CompareLambda((LambdaExpression)x, (LambdaExpression)y);
                case ExpressionType.New:
                    return this.CompareNew((NewExpression)x, (NewExpression)y);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.CompareNewArray((NewArrayExpression)x, (NewArrayExpression)y);
                case ExpressionType.Invoke:
                    return this.CompareInvocation((InvocationExpression)x, (InvocationExpression)y);
                case ExpressionType.MemberInit:
                    return this.CompareMemberInit((MemberInitExpression)x, (MemberInitExpression)y);
                case ExpressionType.ListInit:
                    return this.CompareListInit((ListInitExpression)x, (ListInitExpression)y);
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", x.NodeType));
            }
        }

        protected virtual int CompareListInit(ListInitExpression x, ListInitExpression y)
        {
            int result = this.CompareElementInitializerList(x.Initializers, y.Initializers);
            if (result != 0) return result;

            return this.CompareNew(x.NewExpression, y.NewExpression);
        }

        protected int CompareElementInitializerList(ReadOnlyCollection<ElementInit> x, ReadOnlyCollection<ElementInit> y)
        {
            int result = x.Count - y.Count;
            if (result != 0) return result;

            for (int i = 0; i < x.Count; i++)
            {
                result = this.CompareElementInitializer(x[i], y[i]);
                if (result != 0) return result;
            }

            return 0;
        }

        protected virtual int CompareElementInitializer(ElementInit x, ElementInit y)
        {
            int result = this.CompareMemberInfo(x.AddMethod, y.AddMethod);
            if (result != 0) return result;

            return this.CompareExpressionList(x.Arguments, y.Arguments);
        }

        protected virtual int CompareMemberInit(MemberInitExpression x, MemberInitExpression y)
        {
            int result = this.CompareNew(x.NewExpression, y.NewExpression);
            if (result != 0) return result;

            return this.CompareBindingList(x.Bindings, y.Bindings);
        }

        protected virtual int CompareBindingList(ReadOnlyCollection<MemberBinding> x, ReadOnlyCollection<MemberBinding> y)
        {
            int result = x.Count - y.Count;
            if (result != 0) return result;

            for (int i = 0; i < x.Count; i++)
            {
                result = this.CompareBinding(x[i], y[i]);
                if (result != 0) return result;
            }

            return 0;
        }

        protected virtual int CompareBinding(MemberBinding x, MemberBinding y)
        {
            int result = x.BindingType - y.BindingType;
            if (result != 0) return result;

            return this.CompareMemberInfo(x.Member, y.Member);
        }

        protected virtual int CompareInvocation(InvocationExpression x, InvocationExpression y)
        {
            int result = this.CompareExpressionList(x.Arguments, y.Arguments);
            if (result != 0) return result;

            return this.Compare(x.Expression, y.Expression);
        }

        protected virtual int CompareNewArray(NewArrayExpression x, NewArrayExpression y)
        {
            return this.CompareExpressionList(x.Expressions, y.Expressions);
        }

        protected virtual int CompareNew(NewExpression x, NewExpression y)
        {
            int result;
            if (this.CompareNull(x.Members, y.Members, out result)) return result;

            result = this.CompareMemberInfo(x.Constructor, y.Constructor);
            if (result != 0) return result;

            result = x.Members.Count - y.Members.Count;
            for (int i = 0; i < x.Members.Count; i++)
            {
                result = this.CompareMemberInfo(x.Members[i], y.Members[i]);
                if (result != 0) return result;
            }

            return this.CompareExpressionList(x.Arguments, y.Arguments);
        }

        protected virtual int CompareLambda(LambdaExpression x, LambdaExpression y)
        {
            int result = x.Parameters.Count - y.Parameters.Count;
            if (result != 0) return result;

            for (int i = 0; i < x.Parameters.Count; i++)
            {
                result = this.CompareParameter(x.Parameters[i], y.Parameters[i]);
                if (result != 0) return result;
            }

            return this.Compare(x.Body, y.Body);
        }

        protected virtual int CompareMethodCall(MethodCallExpression x, MethodCallExpression y)
        {
            int result = this.CompareMemberInfo(x.Method, y.Method);
            if (result != 0) return result;

            result = this.CompareExpressionList(x.Arguments, y.Arguments);
            if (result != 0) return result;

            return this.Compare(x.Object, y.Object);
        }

        protected virtual int CompareExpressionList(ReadOnlyCollection<System.Linq.Expressions.Expression> x, ReadOnlyCollection<System.Linq.Expressions.Expression> y)
        {
            int result = x.Count - y.Count;
            if (result != 0) return result;

            for (int i = 0; i < x.Count; i++)
            {
                result = this.Compare(x[i], y[i]);
                if (result != 0) return result;
            }

            return 0;
        }

        protected virtual int CompareMemberAccess(MemberExpression x, MemberExpression y)
        {
            int result = this.CompareMemberInfo(x.Member, y.Member);
            if (result != 0) return result;

            return this.Compare(x.Expression, y.Expression);
        }

        protected virtual int CompareParameter(ParameterExpression x, ParameterExpression y)
        {
            return x.Name.CompareTo(y.Name);
        }

        protected virtual int CompareConstant(ConstantExpression x, ConstantExpression y)
        {
            return Comparer.Default.Compare(x.Value, y.Value);
        }

        protected virtual int CompareConditional(ConditionalExpression x, ConditionalExpression y)
        {
            int result = this.Compare(x.Test, y.Test);
            if (result != 0) return result;

            result = this.Compare(x.IfTrue, y.IfTrue);
            if (result != 0) return result;

            return this.Compare(x.IfFalse, y.IfFalse);
        }

        protected virtual int CompareTypeIs(TypeBinaryExpression x, TypeBinaryExpression y)
        {
            int result = this.CompareType(x.TypeOperand, y.TypeOperand);
            if (result != 0) return result;

            return this.Compare(x.Expression, y.Expression);
        }

        protected virtual int CompareBinary(BinaryExpression x, BinaryExpression y)
        {
            int result = x.IsLifted.CompareTo(y.IsLifted);
            if (result != 0) return result;

            result = x.IsLiftedToNull.CompareTo(y.IsLiftedToNull);
            if (result != 0) return result;

            result = this.CompareMemberInfo(x.Method, y.Method);
            if (result != 0) return result;

            result = this.Compare(x.Left, y.Left);
            if (result != 0) return result;

            result = this.Compare(x.Right, y.Right);
            if (result != 0) return result;

            return this.Compare(x.Conversion, y.Conversion);
        }

        protected virtual int CompareUnary(UnaryExpression x, UnaryExpression y)
        {
            int result = x.IsLifted.CompareTo(y.IsLifted);
            if (result != 0) return result;

            result = x.IsLiftedToNull.CompareTo(y.IsLiftedToNull);
            if (result != 0) return result;

            result = this.CompareMemberInfo(x.Method, y.Method);
            if (result != 0) return result;

            return this.Compare(x.Operand, y.Operand);
        }
    }
}
