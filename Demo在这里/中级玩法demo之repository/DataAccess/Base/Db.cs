using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dos.ORM;

namespace DataAccess.Base
{
    public class Db
    {
        public static readonly DbSession Context = new DbSession("MySqlConn");
        //public static readonly DbSession dbSql = new DbSession(DatabaseType.SqlServer9, @"server=.\sql2008r2;uid=sa;pwd=sa;database=OrchardITdos;");
        //public static readonly DbSession dbAbc = new DbSession(DatabaseType.MySql, @"Data Source=127.0.0.1;Database=ABC;User Id=root;Password=root;Convert Zero Datetime=True;Allow Zero Datetime=True;");
        static Db()
        {
            Context.RegisterSqlLogger(delegate(string sql)
            {
                //在此可以记录sql日志
                //写日志会影响性能，建议开发版本记录sql以便调试，发布正式版本不要记录
                //LogHelper.Debug(sql, "SQL日志");
            });
            //dbSql.RegisterSqlLogger(delegate(string sql)
            //{
            //    //在此可以记录sql日志
            //    //写日志会影响性能，建议开发版本记录sql以便调试，发布正式版本不要记录
            //    //LogHelper.Debug(sql, "SQL日志");
            //}); dbAbc.RegisterSqlLogger(delegate(string sql)
            //{
            //    //在此可以记录sql日志
            //    //写日志会影响性能，建议开发版本记录sql以便调试，发布正式版本不要记录
            //    //LogHelper.Debug(sql, "SQL日志");
            //});
        }
    }
}
