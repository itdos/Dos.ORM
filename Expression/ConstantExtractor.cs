using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Dos.ORM
{
    public class ConstantExtractor : ExpressionVisitor
    {
        private List<object> m_constants;

        public List<object> Extract(System.Linq.Expressions.Expression exp)
        {
            this.m_constants = new List<object>();
            this.Visit(exp);
            return this.m_constants;
        }

        protected override System.Linq.Expressions.Expression VisitConstant(ConstantExpression c)
        {
            this.m_constants.Add(c.Value);
            return c;
        }
    }        
}
