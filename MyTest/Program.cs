using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dos.ORM;
using ITdos.Project.DataModel;

namespace MyTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new DbSession(DatabaseType.MySql, "Data Source=127.0.0.1;Database=ITdos;User Id=root;Password=root;Convert Zero Datetime=True;Allow Zero Datetime=True;");
            var result = db.From<SysUser>().Top(10).ToList();
            var model = result[0];
            model.AttachAll();
            model.Id = Guid.NewGuid();
            model.Test = new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var result1 = db.Insert<SysUser>(model);



            //var db = new DbSession(DatabaseType.SqlServer9, "server=47.93.101.25;database=hMailServer1;uid=sa;pwd=onlyone2;");
            //var result =  db.From<Hmaccounts>().Top(10).ToList();
            //var model = result[0];
            //model.AttachAll();
            //model.Accountaddress = "mail_" + new Random().Next(10,10000) + "@qq.com";
            //var result1 = db.Insert<Hmaccounts>(model);
        }
    }
}
