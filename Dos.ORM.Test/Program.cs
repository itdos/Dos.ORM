using Dos.Common;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dos.ORM.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new DbSession(DatabaseType.MySql, "Data Source=127.0.0.1;Database=ITdos;User Id=root;Password=root;Convert Zero Datetime=True;Allow Zero Datetime=True;");
            db.RegisterSqlLogger(SqlOg);
           var list20160429 = db.From<CmsNews>().Where(d => d.Code == "CSharp").Page(10, 745).ToList();
        }
        static void SqlOg(string sql)
        {
            LogHelper.Debug(sql, "SQL日志_");
        }
    }
}
