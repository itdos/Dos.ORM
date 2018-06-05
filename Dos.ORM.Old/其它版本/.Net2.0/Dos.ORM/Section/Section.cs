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
using System.Text;
using System.Data.Common;
using Dos.ORM;
using Dos.ORM.Common;
using Dos.ORM.Common;
using System.Data;


namespace Dos.ORM
{

    /// <summary>
    /// Section
    /// </summary>
    public abstract class Section
    {

        protected DbSession dbSession;
        protected DbCommand cmd;
        protected DbTransaction tran = null;

        public Section(DbSession dbSession)
        {
            Check.Require(dbSession, "dbSession", Check.NotNullOrEmpty);
            this.dbSession = dbSession;
        }

        #region 执行

        /// <summary>
        /// 返回单个值
        /// </summary>
        /// <returns></returns>
        public virtual object ToScalar()
        {
            return (tran == null ? this.dbSession.ExecuteScalar(cmd) : this.dbSession.ExecuteScalar(cmd, tran));
        }


        /// <summary>
        /// 返回单个值
        /// </summary>
        /// <returns></returns>
        public TResult ToScalar<TResult>()
        {
            return DataUtils.ConvertValue<TResult>(ToScalar());
        }


        /// <summary>
        /// 返回单个实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public TEntity ToFirst<TEntity>()
            where TEntity : Entity
        {
            TEntity t = null;
            using (IDataReader reader = ToDataReader())
            {
                var tempt = EntityUtils.Mapper.Map<TEntity>(reader);
                if (tempt.Count > 0)
                {
                    t = tempt[0];
                }
                #region 2015-08-10注释
                //if (reader.Read())
                //{
                //    t = DataUtils.Create<TEntity>();
                //    t.SetPropertyValues(reader);
                //}
                #endregion
            }
            return t;
        }

        /// <summary>
        /// 返回单个实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public TEntity ToFirstDefault<TEntity>()
            where TEntity : Entity
        {
            TEntity t = ToFirst<TEntity>();

            if (t == null)
                t = DataUtils.Create<TEntity>();

            return t;
        }


        /// <summary>
        /// 返回实体列表
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public List<TEntity> ToList<TEntity>() //where TEntity : Entity
        {
            List<TEntity> listT = new List<TEntity>();
            using (IDataReader reader = ToDataReader())
            {
                listT = EntityUtils.Mapper.Map<TEntity>(reader);
                reader.Close();
            }
            return listT;
        }


        /// <summary>
        /// 返回DataReader
        /// </summary>
        /// <returns></returns>
        public virtual IDataReader ToDataReader()
        {
            return (tran == null ? this.dbSession.ExecuteReader(cmd) : this.dbSession.ExecuteReader(cmd, tran));
        }

        /// <summary>
        /// 返回DataSet
        /// </summary>
        /// <returns></returns>
        public virtual DataSet ToDataSet()
        {
            return (tran == null ? this.dbSession.ExecuteDataSet(cmd) : this.dbSession.ExecuteDataSet(cmd, tran));
        }


        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable ToDataTable()
        {
            return this.ToDataSet().Tables[0];
        }

        /// <summary>
        /// 执行ExecuteNonQuery
        /// </summary>
        /// <returns></returns>
        public virtual int ExecuteNonQuery()
        {
            return (tran == null ? this.dbSession.ExecuteNonQuery(cmd) : this.dbSession.ExecuteNonQuery(cmd, tran));
        }


        #endregion

    }
}

