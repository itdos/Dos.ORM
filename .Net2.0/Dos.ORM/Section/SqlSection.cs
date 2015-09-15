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
using System.Data;
using Dos.ORM;

namespace Dos.ORM
{
    /// <summary>
    /// 执行sql语句
    /// </summary>
    public class SqlSection : Section
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbSession"></param>
        /// <param name="sql"></param>
        public SqlSection(DbSession dbSession, string sql)
            : base(dbSession)
        {

            Check.Require(sql, "sql", Check.NotNullOrEmpty);

            this.cmd = dbSession.Db.GetSqlStringCommand(sql);
        }

        /// <summary>
        /// 设置事务
        /// </summary>
        /// <param name="tran"></param>
        /// <returns></returns>
        public SqlSection SetDbTransaction(DbTransaction tran)
        {
            this.tran = tran;
            return this;
        }

        #region 添加参数


        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"
        /// <param name="dbType"></param>
        /// <returns></returns>
        public SqlSection AddParameter(params DbParameter[] parameters)
        {
            dbSession.Db.AddParameter(this.cmd, parameters);
            return this;
        }


        /// <summary>
        /// 添加参数
        /// </summary>
        public SqlSection AddInParameter(string parameterName, DbType dbType, object value)
        {
            return AddInParameter(parameterName, dbType, 0, value);
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"
        /// <param name="dbType"></param>
        /// <returns></returns>
        public SqlSection AddInParameter(string parameterName, DbType dbType, int size, object value)
        {
            Check.Require(parameterName, "parameterName", Check.NotNullOrEmpty);
            Check.Require(dbType, "dbType", Check.NotNullOrEmpty);

            dbSession.Db.AddInParameter(this.cmd, parameterName, dbType, size, value);
            return this;
        }

        #endregion



    }
}
