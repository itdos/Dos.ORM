using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Dos.ORM
{
    public class Evaluator : IEvaluator
    {
        public object Eval(System.Linq.Expressions.Expression exp)
        {
            if (exp.NodeType == ExpressionType.Constant)
            {
                return ((ConstantExpression)exp).Value;
            }

            LambdaExpression lambda = System.Linq.Expressions.Expression.Lambda(exp);
            Delegate fn = lambda.Compile();

            return fn.DynamicInvoke(null);
        }
    }
}
