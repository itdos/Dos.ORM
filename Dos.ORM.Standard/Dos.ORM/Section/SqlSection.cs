#region << 版 本 注 释 >>
/****************************************************
* 文 件 名：
* Copyright(c) ITdos
* CLR 版本: 4.0.30319.18408
* 创 建 人：steven hu
* 电子邮箱：
* 官方网站：www.ITdos.com
* 创建日期：2010/2/10
* 文件描述：
******************************************************
* 修 改 人：ITdos
* 修改日期：
* 备注描述：
*******************************************************/
#endregion

using System.Data.Common;
using System.Data;

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
