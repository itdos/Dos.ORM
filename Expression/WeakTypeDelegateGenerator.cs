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
