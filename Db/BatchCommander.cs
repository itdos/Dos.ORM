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
using System.Text;
using Dos;
using Dos.ORM;

namespace Dos.ORM
{
    /// <summary>
    /// BatchCommander is used to execute batch queries.
    /// </summary>
    public sealed class BatchCommander
    {
        #region Private Members

        private Database db;
        private int batchSize;
        private DbTransaction tran;
        private List<DbCommand> batchCommands;
        private bool isUsingOutsideTransaction = false;

        private DbCommand MergeCommands()
        {
            DbCommand cmd = db.GetSqlStringCommand("init");
            StringBuilder sb = new StringBuilder();
            foreach (DbCommand item in batchCommands)
            {
                if (item.CommandType == CommandType.Text)
                {
                    foreach (DbParameter dbPara in item.Parameters)
                    {
                        DbParameter p = (DbParameter)((ICloneable)dbPara).Clone();
                        cmd.Parameters.Add(p);
                    }
                    sb.Append(item.CommandText);
                    sb.Append(";");
                }
            }

            if (sb.Length > 0)
            {
                if (db.DbProvider is Dos.ORM.Oracle.OracleProvider)
                {
                    sb.Insert(0, "begin ");
                    sb.Append(" end;");
                }
            }

            cmd.CommandText = sb.ToString();
            return cmd;
        }

        #endregion

        #region Public Members


        /// <summary>
        /// Ö´ÐÐ
        /// </summary>
        public void ExecuteBatch()
        {
            DbCommand cmd = MergeCommands();

            if (cmd.CommandText.Trim().Length > 0)
            {
                if (tran != null)
                {
                    cmd.Connection = tran.Connection;
                    cmd.Transaction = tran;

                }
                else
                {
                    cmd.Connection = db.GetConnection();
                }

                db.DbProvider.PrepareCommand(cmd);

                db.WriteLog(cmd);

                cmd.ExecuteNonQuery();
            }

            batchCommands.Clear();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchCommander"/> class.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="batchSize">Size of the batch.</param>
        /// <param name="il">The il.</param>
        public BatchCommander(Database db, int batchSize, IsolationLevel il)
            : this(db, batchSize, db.BeginTransaction(il))
        {
            isUsingOutsideTransaction = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchCommander"/> class.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="batchSize">Size of the batch.</param>
        /// <param name="tran">The tran.</param>
        public BatchCommander(Database db, int batchSize, DbTransaction tran)
        {
            Check.Require(db != null, "db could not be null.");
            Check.Require(batchSize > 0, "Arguments error - batchSize should > 0.");

            this.db = db;
            this.batchSize = batchSize;
            batchCommands = new List<DbCommand>(batchSize);
            this.tran = tran;
            if (tran != null)
            {
                isUsingOutsideTransaction = true;
            }

        }



        /// <summary>
        /// Initializes a new instance of the <see cref="BatchCommander"/> class.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="batchSize">Size of the batch.</param>
        public BatchCommander(Database db, int batchSize)
            : this(db, batchSize, db.BeginTransaction())
        {
            isUsingOutsideTransaction = false;
        }

        /// <summary>
        /// Processes the specified CMD.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        public void Process(DbCommand cmd)
        {
            if (cmd == null)
            {
                return;
            }

            cmd.Transaction = null;
            cmd.Connection = null;


            batchCommands.Add(cmd);

            if (!db.DbProvider.SupportBatch || batchCommands.Count >= batchSize)
            {
                try
                {
                    ExecuteBatch();
                }
                catch
                {
                    if (tran != null && (!isUsingOutsideTransaction))
                    {
                        tran.Rollback();
                    }

                    throw;
                }
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            try
            {
                ExecuteBatch();

                if (tran != null && (!isUsingOutsideTransaction))
                {
                    tran.Commit();
                }
            }
            catch
            {
                if (tran != null && (!isUsingOutsideTransaction))
                {
                    tran.Rollback();
                }

                throw;
            }
            finally
            {
                if (tran != null && (!isUsingOutsideTransaction))
                {
                    db.CloseConnection(tran);
                }
            }
        }

        #endregion
    }
}
