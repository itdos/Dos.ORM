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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace Dos.ORM
{
    public abstract class ExpressionVisitor
    {
        protected ExpressionVisitor() { }

        protected virtual System.Linq.Expressions.Expression Visit(System.Linq.Expressions.Expression exp)
        {
            if (exp == null)
                return exp;
            switch (exp.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return this.VisitUnary((UnaryExpression)exp);
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
                    return this.VisitBinary((BinaryExpression)exp);
                case ExpressionType.TypeIs:
                    return this.VisitTypeIs((TypeBinaryExpression)exp);
                case ExpressionType.Conditional:
                    return this.VisitConditional((ConditionalExpression)exp);
                case ExpressionType.Constant:
                    return this.VisitConstant((ConstantExpression)exp);
                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression)exp);
                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression)exp);
                case ExpressionType.Call:
                    return this.VisitMethodCall((MethodCallExpression)exp);
                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression)exp);
                case ExpressionType.New:
                    return this.VisitNew((NewExpression)exp);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.VisitNewArray((NewArrayExpression)exp);
                case ExpressionType.Invoke:
                    return this.VisitInvocation((InvocationExpression)exp);
                case ExpressionType.MemberInit:
                    return this.VisitMemberInit((MemberInitExpression)exp);
                case ExpressionType.ListInit:
                    return this.VisitListInit((ListInitExpression)exp);
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
            }
        }

        protected virtual MemberBinding VisitBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return this.VisitMemberAssignment((MemberAssignment)binding);
                case MemberBindingType.MemberBinding:
                    return this.VisitMemberMemberBinding((MemberMemberBinding)binding);
                case MemberBindingType.ListBinding:
                    return this.VisitMemberListBinding((MemberListBinding)binding);
                default:
                    throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType));
            }
        }

        protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
        {
            ReadOnlyCollection<System.Linq.Expressions.Expression> arguments = this.VisitExpressionList(initializer.Arguments);
            if (arguments != initializer.Arguments)
            {
                return System.Linq.Expressions.Expression.ElementInit(initializer.AddMethod, arguments);
            }
            return initializer;
        }

        protected virtual System.Linq.Expressions.Expression VisitUnary(UnaryExpression u)
        {
            System.Linq.Expressions.Expression operand = this.Visit(u.Operand);
            if (operand != u.Operand)
            {
                return System.Linq.Expressions.Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method);
            }
            return u;
        }

        protected virtual System.Linq.Expressions.Expression VisitBinary(BinaryExpression b)
        {
            System.Linq.Expressions.Expression left = this.Visit(b.Left);
            System.Linq.Expressions.Expression right = this.Visit(b.Right);
            System.Linq.Expressions.Expression conversion = this.Visit(b.Conversion);
            if (left != b.Left || right != b.Right || conversion != b.Conversion)
            {
                if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
                    return System.Linq.Expressions.Expression.Coalesce(left, right, conversion as LambdaExpression);
                else
                    return System.Linq.Expressions.Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
            }
            return b;
        }

        protected virtual System.Linq.Expressions.Expression VisitTypeIs(TypeBinaryExpression b)
        {
            System.Linq.Expressions.Expression expr = this.Visit(b.Expression);
            if (expr != b.Expression)
            {
                return System.Linq.Expressions.Expression.TypeIs(expr, b.TypeOperand);
            }
            return b;
        }

        protected virtual System.Linq.Expressions.Expression VisitConstant(ConstantExpression c)
        {
            return c;
        }

        protected virtual System.Linq.Expressions.Expression VisitConditional(ConditionalExpression c)
        {
            System.Linq.Expressions.Expression test = this.Visit(c.Test);
            System.Linq.Expressions.Expression ifTrue = this.Visit(c.IfTrue);
            System.Linq.Expressions.Expression ifFalse = this.Visit(c.IfFalse);
            if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse)
            {
                return System.Linq.Expressions.Expression.Condition(test, ifTrue, ifFalse);
            }
            return c;
        }

        protected virtual System.Linq.Expressions.Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }

        protected virtual System.Linq.Expressions.Expression VisitMemberAccess(MemberExpression m)
        {
            System.Linq.Expressions.Expression exp = this.Visit(m.Expression);
            if (exp != m.Expression)
            {
                return System.Linq.Expressions.Expression.MakeMemberAccess(exp, m.Member);
            }
            return m;
        }

        protected virtual System.Linq.Expressions.Expression VisitMethodCall(MethodCallExpression m)
        {
            System.Linq.Expressions.Expression obj = this.Visit(m.Object);
            IEnumerable<System.Linq.Expressions.Expression> args = this.VisitExpressionList(m.Arguments);
            if (obj != m.Object || args != m.Arguments)
            {
                return System.Linq.Expressions.Expression.Call(obj, m.Method, args);
            }
            return m;
        }

        protected virtual ReadOnlyCollection<System.Linq.Expressions.Expression> VisitExpressionList(ReadOnlyCollection<System.Linq.Expressions.Expression> original)
        {
            List<System.Linq.Expressions.Expression> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                System.Linq.Expressions.Expression p = this.Visit(original[i]);
                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<System.Linq.Expressions.Expression>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(p);
                }
            }
            if (list != null)
            {
                return list.AsReadOnly();
            }
            return original;
        }

        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            System.Linq.Expressions.Expression e = this.Visit(assignment.Expression);
            if (e != assignment.Expression)
            {
                return System.Linq.Expressions.Expression.Bind(assignment.Member, e);
            }
            return assignment;
        }

        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(binding.Bindings);
            if (bindings != binding.Bindings)
            {
                return System.Linq.Expressions.Expression.MemberBind(binding.Member, bindings);
            }
            return binding;
        }

        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(binding.Initializers);
            if (initializers != binding.Initializers)
            {
                return System.Linq.Expressions.Expression.ListBind(binding.Member, initializers);
            }
            return binding;
        }

        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
        {
            List<MemberBinding> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                MemberBinding b = this.VisitBinding(original[i]);
                if (list != null)
                {
                    list.Add(b);
                }
                else if (b != original[i])
                {
                    list = new List<MemberBinding>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(b);
                }
            }
            if (list != null)
                return list;
            return original;
        }

        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
        {
            List<ElementInit> list = null;
            for (int i = 0, n = original.Count; i < n; i++)
            {
                ElementInit init = this.VisitElementInitializer(original[i]);
                if (list != null)
                {
                    list.Add(init);
                }
                else if (init != original[i])
                {
                    list = new List<ElementInit>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(init);
                }
            }
            if (list != null)
                return list;
            return original;
        }

        protected virtual System.Linq.Expressions.Expression VisitLambda(LambdaExpression lambda)
        {
            System.Linq.Expressions.Expression body = this.Visit(lambda.Body);
            if (body != lambda.Body)
            {
                return System.Linq.Expressions.Expression.Lambda(lambda.Type, body, lambda.Parameters);
            }
            return lambda;
        }

        protected virtual NewExpression VisitNew(NewExpression nex)
        {
            IEnumerable<System.Linq.Expressions.Expression> args = this.VisitExpressionList(nex.Arguments);
            if (args != nex.Arguments)
            {
                if (nex.Members != null)
                    return System.Linq.Expressions.Expression.New(nex.Constructor, args, nex.Members);
                else
                    return System.Linq.Expressions.Expression.New(nex.Constructor, args);
            }
            return nex;
        }

        protected virtual System.Linq.Expressions.Expression VisitMemberInit(MemberInitExpression init)
        {
            NewExpression n = this.VisitNew(init.NewExpression);
            IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings);
            if (n != init.NewExpression || bindings != init.Bindings)
            {
                return System.Linq.Expressions.Expression.MemberInit(n, bindings);
            }
            return init;
        }

        protected virtual System.Linq.Expressions.Expression VisitListInit(ListInitExpression init)
        {
            NewExpression n = this.VisitNew(init.NewExpression);
            IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(init.Initializers);
            if (n != init.NewExpression || initializers != init.Initializers)
            {
                return System.Linq.Expressions.Expression.ListInit(n, initializers);
            }
            return init;
        }

        protected virtual System.Linq.Expressions.Expression VisitNewArray(NewArrayExpression na)
        {
            IEnumerable<System.Linq.Expressions.Expression> exprs = this.VisitExpressionList(na.Expressions);
            if (exprs != na.Expressions)
            {
                if (na.NodeType == ExpressionType.NewArrayInit)
                {
                    return System.Linq.Expressions.Expression.NewArrayInit(na.Type.GetElementType(), exprs);
                }
                else
                {
                    return System.Linq.Expressions.Expression.NewArrayBounds(na.Type.GetElementType(), exprs);
                }
            }
            return na;
        }

        protected virtual System.Linq.Expressions.Expression VisitInvocation(InvocationExpression iv)
        {
            IEnumerable<System.Linq.Expressions.Expression> args = this.VisitExpressionList(iv.Arguments);
            System.Linq.Expressions.Expression expr = this.Visit(iv.Expression);
            if (args != iv.Arguments || expr != iv.Expression)
            {
                return System.Linq.Expressions.Expression.Invoke(expr, args);
            }
            return iv;
        }
    }
}
