using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Entities;

namespace DataCache
{
    public class TableMysqlCache
    {
        public TableMysql GetUserModel(Guid userId)
        {
            var result = CacheBase.Get<TableMysql>("GetUser" + userId);
            return result;
        }
        public bool SetUserModel(Guid userId, TableMysql list)
        {
            return CacheBase.Set("GetUser" + userId, list);
        }
        public bool DelUserModel(Guid userId)
        {
            return CacheBase.Remove("GetUser" + userId);
        }
    }
}
