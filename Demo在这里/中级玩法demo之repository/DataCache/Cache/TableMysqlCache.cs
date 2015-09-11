using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Entities;
using DataCache.Base;

namespace DataCache
{
    public class TableMysqlCache : CacheBase
    {
        public TableMysql GetUserModel(Guid userId)
        {
            var result = Get<TableMysql>("GetUser" + userId);
            return result;
        }
        public bool SetUserModel(TableMysql model)
        {
            return Set("GetUser" + model.Id, model);
        }
        public bool DelUserModel(Guid userId)
        {
            return Remove("GetUser" + userId);
        }
    }
}
