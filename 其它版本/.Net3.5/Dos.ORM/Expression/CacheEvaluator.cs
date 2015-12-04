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
