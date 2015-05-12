#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：ExpToGroupByClip
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Dos.ORM.Common;

namespace Dos.ORM
{
    public static class ExpToSelect<T>
    {
        public static Field[] ToSelect(Expression<Func<T, object>> expr)
        {
            return ToClip(expr.Body);
        }
        private static Field[] ToClip(System.Linq.Expressions.Expression exprBody)
        {
            if (exprBody is MemberExpression)
            {
                var e = (MemberExpression)exprBody;
                return new Field[] { new Field(e.Member.Name, e.Expression.Type.Name) };
            }
            if (exprBody is NewExpression)
            {
                var exNew = (NewExpression)exprBody;
                var type = exNew.Constructor.DeclaringType;
                var list = new List<string>(exNew.Arguments.Count);
                var f = new Field[exNew.Arguments.Count];
                var i = 0;
                foreach (MemberExpression member in exNew.Arguments)
                {
                    f[i] = new Field(member.Member.Name, member.Expression.Type.Name);
                    i++;
                }
                return f;
            }
            throw new Exception("不支持的Select写法！");
        }
    }
}
