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
    }
}
