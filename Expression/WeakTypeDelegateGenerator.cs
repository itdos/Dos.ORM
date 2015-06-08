using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Dos.ORM
{
    public class WeakTypeDelegateGenerator : ExpressionVisitor
    {
        private List<ParameterExpression> m_parameters;

        public Delegate Generate(System.Linq.Expressions.Expression exp)
        {
            this.m_parameters = new List<ParameterExpression>();

            var body = this.Visit(exp);
            var lambda = System.Linq.Expressions.Expression.Lambda(body, this.m_parameters.ToArray());
            return lambda.Compile();
        }

        protected override System.Linq.Expressions.Expression VisitConstant(ConstantExpression c)
        {
            var p = System.Linq.Expressions.Expression.Parameter(c.Type, "p" + this.m_parameters.Count);
            this.m_parameters.Add(p);
            return p;
        }
    }
}
