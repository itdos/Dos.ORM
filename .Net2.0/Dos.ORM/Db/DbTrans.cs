/*************************************************************************
 * 
 * Hxj.Data
 * 
 * 2010-2-10
 * 
 * steven hu   
 *  
 * Support: http://www.cnblogs.com/huxj
 *   
 * 
 * Change History:
 * 
 * 
**************************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Dos;
using Dos.ORM;

namespace Dos.ORM
{
    /// <summary>
    /// 事务
    /// </summary>
    public class DbTrans : IDisposable
    {

        /// <summary>
        /// 事务
        /// </summary>
        private DbTransaction trans;

        /// <summary>
        /// 连接
        /// </summary>
        private DbConnection conn;


        /// <summary>
        /// 
        /// </summary>
        private DbSession dbSession;

        /// <summary>
        /// 判断是否有提交或回滚
        /// </summary>
        private bool isCommitOrRollback = false;

        /// <summary>
        /// 是否关闭
        /// </summary>
        private bool isClose = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="dbSession"></param>
        public DbTrans(DbTransaction trans, DbSession dbSession)
        {
            Check.Require(trans, "trans", Check.NotNull);

            this.trans = trans;
            this.conn = trans.Connection;
            this.dbSession = dbSession;

            if (this.conn.State != ConnectionState.Open)
                this.conn.Open();

        }



        /// <summary>
        /// 连接
        /// </summary>
        public DbConnection Connection
        {
            get
            {
                return conn;
            }
        }

        /// <summary>
        /// 事务级别
        /// </summary>
        public IsolationLevel IsolationLevel
        {
            get { return trans.IsolationLevel; }
        }

        /// <summary>
        /// 提交
        /// </summary>
        public void Commit()
        {
            trans.Commit();

            isCommitOrRollback = true;

            Close();
        }


        /// <summary>
        /// 回滚
        /// </summary>
        public void Rollback()
        {
            trans.Rollback();

            isCommitOrRollback = true;

            Close();
        }


        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="dbTrans"></param>
        /// <returns></returns>
        public static implicit operator DbTransaction(DbTrans dbTrans)
        {
            return dbTrans.trans;
        }


        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            if (isClose)
                return;

            if (!isCommitOrRollback)
            {
                isCommitOrRollback = true;

                trans.Rollback();
            }

            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }

            trans.Dispose();

            isClose = true;
        }


        #region IDisposable 成员
        /// <summary>
        /// 关闭连接并释放资源
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        #endregion


        /// <summary>
        /// FromSql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public SqlSection FromSql(string sql)
        {
            return dbSession.FromSql(sql).SetDbTransaction(trans);
        }


        /// <summary>
        /// FromPro
        /// </summary>
        /// <param name="proName"></param>
        /// <returns></returns>
        public ProcSection FromPro(string proName)
        {
            return dbSession.FromProc(proName).SetDbTransaction(trans);
        }


        #region 查询


        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public FromSection<TEntity> From<TEntity>()
            where TEntity : Entity
        {
            return new FromSection<TEntity>(dbSession.Db, trans);
        }


        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public FromSection From(string tableName)
        {
            return new FromSection(dbSession.Db, tableName, trans);
        }


        #endregion


        #region 更新

        /// <summary>
        /// 更新全部字段  
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        public void UpdateAll<TEntity>(params TEntity[] entities)
            where TEntity : Entity
        {
            dbSession.UpdateAll<TEntity>(trans, entities);
        }


        /// <summary>
        /// 更新全部字段  
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        public int UpdateAll<TEntity>(TEntity entity)
            where TEntity : Entity
        {
            return dbSession.UpdateAll<TEntity>(trans,entity);
        }


        /// <summary>
        /// 更新全部字段
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public int UpdateAll<TEntity>(TEntity entity, WhereClip where)
            where TEntity : Entity
        {
            return dbSession.UpdateAll<TEntity>(trans,entity, where);
        }



        /// <summary>
        /// 更新  
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        public void Update<TEntity>(params TEntity[] entities)
            where TEntity : Entity
        {
            dbSession.Update<TEntity>(trans, entities);
        }


        /// <summary>
        /// 更新  
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        public int Update<TEntity>(TEntity entity)
            where TEntity : Entity
        {
            return dbSession.Update<TEntity>(trans,entity);
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Update<TEntity>(TEntity entity, WhereClip where)
            where TEntity : Entity
        {
            return dbSession.Update<TEntity>(trans,entity, where);
        }


        /// <summary>
        /// 更新单个值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Update<TEntity>(Field field, object value, WhereClip where)
            where TEntity : Entity
        {
            return dbSession.Update<TEntity>(trans,field, value, where);
        }




        /// <summary>
        /// 更新多个值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fieldValue"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Update<TEntity>(Dictionary<Field, object> fieldValue, WhereClip where)
              where TEntity : Entity
        {
            return dbSession.Update<TEntity>(trans,fieldValue, where);
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Update<TEntity>(Field[] fields, object[] values, WhereClip where)
            where TEntity : Entity
        {
            return dbSession.Update<TEntity>(trans,fields, values, where);
        }



        #endregion


        #region 删除

        /// <summary>
        ///  删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Delete<TEntity>(TEntity entity)
            where TEntity : Entity
        {
            return dbSession.Delete<TEntity>(trans,entity);
        }



        /// <summary>
        ///  删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="pkValues"></param>
        /// <returns></returns>
        public int Delete<TEntity>(params object[] pkValues)
            where TEntity : Entity
        {
            return dbSession.DeleteByPrimaryKey<TEntity>(trans, pkValues);
        }

        /// <summary>
        ///  删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="pkValues"></param>
        /// <returns></returns>
        public int Delete<TEntity>(params string[] pkValues)
            where TEntity : Entity
        {
            return dbSession.DeleteByPrimaryKey<TEntity>(trans, pkValues);
        }


        /// <summary>
        ///  删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        public int Delete<TEntity>(WhereClip where)
            where TEntity : Entity
        {
            return dbSession.Delete<TEntity>(trans,where);
        }

        #endregion


        #region 添加

        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public void Insert<TEntity>(params TEntity[] entities)
            where TEntity : Entity
        {
            dbSession.Insert<TEntity>(entities);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Insert<TEntity>(TEntity entity)
            where TEntity : Entity
        {
            return dbSession.Insert<TEntity>(trans,entity);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fields"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public int Insert<TEntity>(Field[] fields, object[] values)
            where TEntity : Entity
        {
            return dbSession.Insert<TEntity>(trans,fields, values);
        }



        #endregion
    }
}
