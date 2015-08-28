using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Entities;

namespace DataCache
{
    public class TableMysqlCache
    {
        public List<TableMysql> GetExamCost(Guid examRoomId)
        {
            var result = CacheBase.Get<List<TableMysql>>("GetExamCost_" + examRoomId);
            return result;
        }
        public bool SetExamCost(Guid examRoomId, List<TableMysql> list)
        {
            return CacheBase.Set("GetExamCost_" + examRoomId, list);
        }
        public bool DelExamCost(Guid examRoomId)
        {
            return CacheBase.Remove("GetExamCost_" + examRoomId);
        }
    }
}
