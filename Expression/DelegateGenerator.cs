using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Dos.ORM
{
    public class DelegateGenerator : ExpressionVisitor
    {
        private static readonly MethodInfo s_indexerInfo = typeof(List<object>).GetMethod("get_Item");

        private int m_parameterCount;
        private ParameterExpression m_parametersExpression;

        public Func<List<object>, object> Generate(System.Linq.Expressions.Expression exp)
        {
            this.m_parameterCount = 0;
            this.m_parametersExpression =
                System.Linq.Expressions.Expression.Parameter(typeof(List<object>), "parameters");

            var body = this.Visit(exp); // normalize
            if (body.Type != typeof(object))
            {
                body = System.Linq.Expressions.Expression.Convert(body, typeof(object));
            }

            var lambda = System.Linq.Expressions.Expression.Lambda<Func<List<object>, object>>(body, this.m_parametersExpression);
            return lambda.Compile();
        }

        protected override System.Linq.Expressions.Expression VisitConstant(ConstantExpression c)
        {
            System.Linq.Expressions.Expression exp = System.Linq.Expressions.Expression.Call(
                this.m_parametersExpression,
                s_indexerInfo,
                System.Linq.Expressions.Expression.Constant(this.m_parameterCount++));
            return c.Type == typeof(object) ? exp : System.Linq.Expressions.Expression.Convert(exp, c.Type);
        }
    }
}
