using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dos.ORM;

namespace Business
{
    /// <summary>
    /// 
    /// </summary>
    public class DB
    {
        public static readonly DbSession Context = new DbSession("DosConn");
        static DB()
        {
            Context.RegisterSqlLogger(delegate(string sql)
            {
                //在此可以记录sql日志
                //写日志会影响性能，建议开发版本记录sql以便调试，发布正式版本不要记录
            });
        }
    }
}
