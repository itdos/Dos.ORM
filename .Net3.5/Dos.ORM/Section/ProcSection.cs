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
    /// 执行存储过程
    /// </summary>
    public class ProcSection : Section
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbSession"></param>
        /// <param name="procName"></param>
        public ProcSection(DbSession dbSession, string procName)
            : base(dbSession)
        {
            Check.Require(procName, "procName", Check.NotNullOrEmpty);
            this.cmd = dbSession.Db.GetStoredProcCommand(procName);
        }

        /// <summary>
        /// 返回的参数
        /// </summary>
        private List<string> outParameters = new List<string>();

        /// <summary>
        /// 设置事务
        /// </summary>
        /// <param name="tran"></param>
        /// <returns></returns>
        public ProcSection SetDbTransaction(DbTransaction tran)
        {
            this.tran = tran;
            return this;
        }

        /// <summary>
        /// 存储过程参数不要加前缀
        /// </summary>
        protected bool isParameterSpecial
        {
            get
            {
                return !(dbSession.Db.DbProvider is SqlServer.SqlServerProvider
                           || dbSession.Db.DbProvider is SqlServer9.SqlServer9Provider
                           || dbSession.Db.DbProvider is MsAccess.MsAccessProvider);
            }
        }

        /// <summary>
        /// 获取参数名字
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        protected string getParameterName(string parameterName)
        {
            Check.Require(parameterName, "parameterName", Check.NotNullOrEmpty);

            if (!isParameterSpecial)
            {
                return dbSession.Db.DbProvider.BuildParameterName(parameterName);
            }
            else
            {
                return parameterName.TrimStart(dbSession.Db.DbProvider.ParamPrefix);
            }


        }
        /// <summary>
        /// 返回存储过程返回值
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetReturnValues()
        {
            Dictionary<string, object> returnValues = new Dictionary<string, object>();
            foreach (string outParameter in outParameters)
            {
                returnValues.Add(outParameter, cmd.Parameters[getParameterName(outParameter)].Value);
            }
            return returnValues;
        }

        #region 添加参数


        /// <summary>
        /// 添加参数
        /// </summary>
        public ProcSection AddParameter(params DbParameter[] parameters)
        {
            dbSession.Db.AddParameter(this.cmd, parameters);
            return this;
        }


        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public ProcSection AddInParameter(string parameterName, DbType dbType, object value)
        {
            return AddInParameter(parameterName, dbType, 0, value);
        }

        /// <summary>
        /// 添加输入参数
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public ProcSection AddInParameter(string parameterName, DbType dbType, int size, object value)
        {
            Check.Require(parameterName, "parameterName", Check.NotNullOrEmpty);
            Check.Require(dbType, "dbType", Check.NotNullOrEmpty);

            dbSession.Db.AddInParameter(this.cmd, parameterName, dbType, size, value);
            return this;
        }

        /// <summary>
        /// 添加输出参数
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public ProcSection AddOutParameter(string parameterName, DbType dbType)
        {
            return AddOutParameter(parameterName, dbType, 0);
        }

        /// <summary>
        /// 添加输出参数
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ProcSection AddOutParameter(string parameterName, DbType dbType, int size)
        {
            Check.Require(parameterName, "parameterName", Check.NotNullOrEmpty);
            Check.Require(dbType, "dbType", Check.NotNullOrEmpty);

            dbSession.Db.AddOutParameter(this.cmd, parameterName, dbType, size);
            outParameters.Add(parameterName);
            return this;
        }


        /// <summary>
        /// 添加输入输出参数
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ProcSection AddInputOutputParameter(string parameterName, DbType dbType, object value)
        {
            return AddInputOutputParameter(parameterName, dbType, 0, value);
        }


        /// <summary>
        /// 添加输入输出参数
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <param name="value"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ProcSection AddInputOutputParameter(string parameterName, DbType dbType, int size, object value)
        {
            Check.Require(parameterName, "parameterName", Check.NotNullOrEmpty);
            Check.Require(dbType, "dbType", Check.NotNullOrEmpty);

            dbSession.Db.AddInputOutputParameter(this.cmd, parameterName, dbType, size, value);

            outParameters.Add(parameterName);

            return this;
        }

        /// <summary>
        /// 添加返回参数
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public ProcSection AddReturnValueParameter(string parameterName, DbType dbType)
        {
            return AddReturnValueParameter(parameterName, dbType, 0);
        }

        /// <summary>
        /// 添加返回参数
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ProcSection AddReturnValueParameter(string parameterName, DbType dbType, int size)
        {
            Check.Require(parameterName, "parameterName", Check.NotNullOrEmpty);
            Check.Require(dbType, "dbType", Check.NotNullOrEmpty);

            dbSession.Db.AddReturnValueParameter(this.cmd, parameterName, dbType, size);
            outParameters.Add(parameterName);
            return this;
        }

        #endregion


        #region 执行

        /// <summary>
        /// 操作参数名称
        /// </summary>
        protected void executeBefore()
        {
            if (isParameterSpecial)
            {
                if (cmd.Parameters != null && cmd.Parameters.Count > 0)
                {
                    foreach (DbParameter dbpara in cmd.Parameters)
                    {
                        if (!string.IsNullOrEmpty(dbpara.ParameterName))
                        {
                            dbpara.ParameterName = dbpara.ParameterName.TrimStart(dbSession.Db.DbProvider.ParamPrefix);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 返回单个值
        /// </summary>
        /// <returns></returns>
        public override object ToScalar()
        {
            executeBefore();

            return base.ToScalar();
        }

        /// <summary>
        /// 返回DataReader
        /// </summary>
        /// <returns></returns>
        public override IDataReader ToDataReader()
        {
            executeBefore();

            return base.ToDataReader();
        }

        /// <summary>
        /// 返回DataSet
        /// </summary>
        /// <returns></returns>
        public override DataSet ToDataSet()
        {
            executeBefore();

            return base.ToDataSet();
        }


        /// <summary>
        /// 执行ExecuteNonQuery
        /// </summary>
        /// <returns></returns>
        public override int ExecuteNonQuery()
        {
            executeBefore();

            return base.ExecuteNonQuery();
        }

        #endregion

    }
}

