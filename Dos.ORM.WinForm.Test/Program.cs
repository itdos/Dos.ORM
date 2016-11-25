using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dos.ORM.WinForm.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var db20160927 = new DbSession(DatabaseType.MySql, "Data Source=127.0.0.1;Database=ITdos;User Id=root;Password=root;Convert Zero Datetime=True;Allow Zero Datetime=True;");
        }
    }
}
