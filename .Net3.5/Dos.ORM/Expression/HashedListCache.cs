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
using System.Threading;

namespace Dos.ORM
{
    public class HashedListCache<T> : IExpressionCache<T> where T : class
    {
        private Dictionary<int, SortedList<System.Linq.Expressions.Expression, T>> m_storage =
            new Dictionary<int, SortedList<System.Linq.Expressions.Expression, T>>();
        private ReaderWriterLockSlim m_rwLock = new ReaderWriterLockSlim();

        public T Get(System.Linq.Expressions.Expression key, Func<System.Linq.Expressions.Expression, T> creator)
        {
            SortedList<System.Linq.Expressions.Expression, T> sortedList;
            T value;

            int hash = new Hasher().Hash(key);
            this.m_rwLock.EnterReadLock();
            try
            {
                if (this.m_storage.TryGetValue(hash, out sortedList) &&
                    sortedList.TryGetValue(key, out value))
                {
                    return value;
                }
            }
            finally
            {
                this.m_rwLock.ExitReadLock();
            }

            this.m_rwLock.EnterWriteLock();
            try
            {
                if (!this.m_storage.TryGetValue(hash, out sortedList))
                {
                    sortedList = new SortedList<System.Linq.Expressions.Expression, T>(new Comparer());
                    this.m_storage.Add(hash, sortedList);
                }

                if (!sortedList.TryGetValue(key, out value))
                {
                    value = creator(key);
                    sortedList.Add(key, value);
                }
                
                return value;
            }
            finally
            {
                this.m_rwLock.ExitWriteLock();
            }
        }

        private class Hasher : ExpressionHasher
        {
            protected override System.Linq.Expressions.Expression VisitConstant(ConstantExpression c)
            {
                return c;
            }
        }

        internal class Comparer : ExpressionComparer
        {
            protected override int CompareConstant(ConstantExpression x, ConstantExpression y)
            {
                return 0;
            }
        }
    }
}
