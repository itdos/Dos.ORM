using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Dos.ORM
{
    public class CacheEvaluator: IEvaluator
    {
        private static IExpressionCache<Delegate> s_cache = new HashedListCache<Delegate>();

        private WeakTypeDelegateGenerator m_delegateGenerator = new WeakTypeDelegateGenerator();
        private ConstantExtractor m_constantExtrator = new ConstantExtractor();

        private IExpressionCache<Delegate> m_cache;
        private Func<System.Linq.Expressions.Expression, Delegate> m_creatorDelegate;

        public CacheEvaluator()
            : this(s_cache)
        { }

        public CacheEvaluator(IExpressionCache<Delegate> cache)
        {
            this.m_cache = cache;
            this.m_creatorDelegate = (key) => this.m_delegateGenerator.Generate(key);
        }

        public object Eval(System.Linq.Expressions.Expression exp)
        {
            if (exp.NodeType == ExpressionType.Constant)
            {
                return ((ConstantExpression)exp).Value;
            }

            var parameters = this.m_constantExtrator.Extract(exp);
            var func = this.m_cache.Get(exp, this.m_creatorDelegate);
            return func.DynamicInvoke(parameters.ToArray());
        }
    }
}
