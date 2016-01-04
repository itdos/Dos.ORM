using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Entities;
using DataCache.Base;

namespace DataCache
{
    public class TestTableCache : CacheBase
    {
        public static TestTable1 GetUserModel(Guid userId)
        {
            var result = Get<TestTable1>("GetUser" + userId);
            return result;
        }
        public static bool SetUserModel(TestTable1 model)
        {
            return Set("GetUser" + model.Id, model);
        }
        public static bool DelUserModel(Guid userId)
        {
            return Remove("GetUser" + userId);
        }
    }
}
