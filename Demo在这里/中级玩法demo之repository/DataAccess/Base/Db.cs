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
        static Db()
        {
            Context.RegisterSqlLogger(delegate(string sql)
            {
                //在此可以记录sql日志
                //写日志会影响性能，建议开发版本记录sql以便调试，发布正式版本不要记录
                //LogHelper.Debug(sql, "SQL日志");
            });
        }
    }
}
