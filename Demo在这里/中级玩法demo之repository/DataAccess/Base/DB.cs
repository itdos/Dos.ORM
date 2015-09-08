using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dos.Common;
using Dos.ORM;

namespace DataAccess
{
    /// <summary>
    /// 
    /// </summary>
    public class DB
    {
        public static readonly DbSession MySql = new DbSession("MySqlConn");
        public static readonly DbSession SqlServer = new DbSession("SqlServerConn");
        public static readonly DbSession Sqlite = new DbSession("SqliteConn");
        public static readonly DbSession Access = new DbSession("AccessConn");
        public static readonly DbSession Oracle = new DbSession("OracleConn");
        public static readonly DbSession PostgreSql = new DbSession("PostgreSqlConn");

        static DB()
        {
            MySql.RegisterSqlLogger(delegate(string sql)
            {
                //在此可以记录sql日志
                //写日志会影响性能，建议开发版本记录sql以便调试，发布正式版本不要记录
                LogHelper.Debug(sql, "SQL日志");
            });
            SqlServer.RegisterSqlLogger(delegate(string sql)
            {
                //在此可以记录sql日志
                //写日志会影响性能，建议开发版本记录sql以便调试，发布正式版本不要记录
            });
            Sqlite.RegisterSqlLogger(delegate(string sql)
            {
                //在此可以记录sql日志
                //写日志会影响性能，建议开发版本记录sql以便调试，发布正式版本不要记录
            });
            Access.RegisterSqlLogger(delegate(string sql)
            {
                //在此可以记录sql日志
                //写日志会影响性能，建议开发版本记录sql以便调试，发布正式版本不要记录
            });
            Oracle.RegisterSqlLogger(delegate(string sql)
            {
                //在此可以记录sql日志
                //写日志会影响性能，建议开发版本记录sql以便调试，发布正式版本不要记录
            });
            PostgreSql.RegisterSqlLogger(delegate(string sql)
            {
                //在此可以记录sql日志
                //写日志会影响性能，建议开发版本记录sql以便调试，发布正式版本不要记录
            });
        }
    }
}
