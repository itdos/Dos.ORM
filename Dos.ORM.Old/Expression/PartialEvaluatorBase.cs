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

namespace Dos.ORM
{
    public abstract class PartialEvaluatorBase : ExpressionVisitor
    {
        private IEvaluator m_evaluator;
        private HashSet<System.Linq.Expressions.Expression> m_candidates;

        protected PartialEvaluatorBase(IEvaluator evaluator)
        {
            this.m_evaluator = evaluator;
        }

        public System.Linq.Expressions.Expression Eval(System.Linq.Expressions.Expression exp)
        {
            this.m_candidates = new Nominator().Nominate(exp);
            return this.m_candidates.Count > 0 ? this.Visit(exp) : exp;
        }

        protected override System.Linq.Expressions.Expression Visit(System.Linq.Expressions.Expression exp)
        {
            if (exp == null)
            {
                return null;
            }

            if (this.m_candidates.Contains(exp))
            {
                return exp.NodeType == ExpressionType.Constant ? exp :
                    System.Linq.Expressions.Expression.Constant(this.m_evaluator.Eval(exp), exp.Type);
            }

            return base.Visit(exp);
        }

        /// <summary>
        /// Performs bottom-up analysis to determine which nodes can possibly
        /// be part of an evaluated sub-tree.
        /// </summary>
        private class Nominator : ExpressionVisitor
        {
            private Func<System.Linq.Expressions.Expression, bool> m_fnCanBeEvaluated;
            private HashSet<System.Linq.Expressions.Expression> m_candidates;
            private bool m_cannotBeEvaluated;

            public Nominator()
                : this(CanBeEvaluatedLocally)
            { }

            public Nominator(Func<System.Linq.Expressions.Expression, bool> fnCanBeEvaluated)
            {
                this.m_fnCanBeEvaluated = fnCanBeEvaluated ?? CanBeEvaluatedLocally;
            }

            private static bool CanBeEvaluatedLocally(System.Linq.Expressions.Expression exp)
            {
                return exp.NodeType != ExpressionType.Parameter;
            }

            internal HashSet<System.Linq.Expressions.Expression> Nominate(System.Linq.Expressions.Expression expression)
            {
                this.m_candidates = new HashSet<System.Linq.Expressions.Expression>();
                this.Visit(expression);
                return this.m_candidates;
            }

            protected override System.Linq.Expressions.Expression Visit(System.Linq.Expressions.Expression expression)
            {
                if (expression != null)
                {
                    bool saveCannotBeEvaluated = this.m_cannotBeEvaluated;
                    this.m_cannotBeEvaluated = false;

                    base.Visit(expression);

                    if (!this.m_cannotBeEvaluated)
                    {
                        if (this.m_fnCanBeEvaluated(expression))
                        {
                            this.m_candidates.Add(expression);
                        }
                        else
                        {
                            this.m_cannotBeEvaluated = true;
                        }
                    }

                    this.m_cannotBeEvaluated |= saveCannotBeEvaluated;
                }

                return expression;
            }
        }
    }
}
