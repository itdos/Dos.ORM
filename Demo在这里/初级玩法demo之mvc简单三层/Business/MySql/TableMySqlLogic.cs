using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dos.Common;
using Dos.ORM;
using Model;
using Model.Base;
using System.Data.Common;

namespace Business
{
    public class TableMySqlLogic
    {
        /// <summary>
        /// 获取数据。
        /// </summary>
        public BaseResult GetUser(TestTableParam param)
        {
            #region 测试子查询修改
            //var model = new TestTable
            //{
            //    IDNumber = "XXXXXXXXXX"
            //};
            //var count2 = DB.MySql.Update<TestTable>(model, TestTable._.Id.SubQueryIn(
            //    DB.MySql.From<TestTable>().Select(d => d.Id).Where(d => d.IDNumber == "777")
            //));
            ////以上同Sql语句：
            ////update TestTable  set IDNumber='XXXXXXX' where Id in 
            ////              (SELECT Id from TestTable where IDNumber='777')
            #endregion
            #region 测试批量Save
            //var listModel = new List<TestTable>();
            //var model1 = new TestTable()
            //{
            //    Id = Guid.NewGuid(),
            //    IDNumber = "0000",
            //    CreateTime = DateTime.Now,
            //    MobilePhone = "000",
            //    Name = "00000"
            //};
            //var model2 = new TestTable()
            //{
            //    Id = Guid.Parse("fdc87fad-0e80-49b2-aab0-c52d1fcd1297"),
            //    IDNumber = "000",
            //    CreateTime = DateTime.Now,
            //    MobilePhone = "000",
            //    Name = "00000"
            //};
            //var model3 = new TestTable()
            //{
            //    Id = Guid.Parse("68805e30-5bc4-43ae-8ad7-8464be215e69")
            //};
            //model1.Attach(EntityState.Added);
            //model2.Attach(EntityState.Modified);
            //model3.Attach(EntityState.Deleted);
            //listModel.Add(model1);
            //listModel.Add(model2);
            //listModel.Add(model3);
            //var count = DB.MySql.Save<TestTable>(listModel);
            #endregion
            var where = new Where<TableMysql>();
            #region 模糊搜索条件
            if (!string.IsNullOrWhiteSpace(param.SearchName))
            {
                where.And(d => d.Name.Like(param.SearchName));
            }
            if (!string.IsNullOrWhiteSpace(param.SearchIDNumber))
            {
                where.And(d => d.IDNumber.Like(param.SearchIDNumber));
            }
            if (!string.IsNullOrWhiteSpace(param.SearchMobilePhone))
            {
                where.And(d => d.MobilePhone.Like(param.SearchMobilePhone));
            }
            #endregion
            var fs = DB.MySql.From<TableMysql>()
                .Where(where)
                .OrderByDescending(d => d.CreateTime);
            #region 是否分页
            var dateCount = 0;
            if (param.pageIndex != null && param.pageSize != null)
            {
                //取总数，以计算共多少页。自行考虑将总数缓存。
                dateCount = fs.Count();//.SetCacheTimeOut(10)
                fs.Page(param.pageSize.Value, param.pageIndex.Value);
            }
            #endregion
            var list = fs.ToList();
            return new BaseResult(true, list, "", dateCount);
        }
        /// <summary>
        /// 新增数据。必须传入姓名Name，手机号MobilePhone，身份证号IDNumber
        /// </summary>
        public BaseResult AddUser(TestTableParam param)
        {
            if (string.IsNullOrWhiteSpace(param.Name) || string.IsNullOrWhiteSpace(param.MobilePhone)
                    || string.IsNullOrWhiteSpace(param.IDNumber))
            {
                return new BaseResult(false, null, "参数错误！");
            }
            var model = new TableMysql
            {
                Id = Guid.NewGuid(),
                Name = param.Name,
                IDNumber = param.IDNumber,
                MobilePhone = param.MobilePhone,
                CreateTime = DateTime.Now
            };
            var count = DB.MySql.Insert<TableMysql>(model);
            return new BaseResult(count > 0, count, count > 0 ? "" : "数据库受影响行数为0！");
        }
        /// <summary>
        /// 删除数据。必须传入Id
        /// </summary>
        public BaseResult DelUser(TestTableParam param)
        {
            if (param.Id == null)
            {
                return new BaseResult(false, null, "参数错误！");
            }
            var count = DB.MySql.Delete<TableMysql>(d => d.Id == param.Id);
            return new BaseResult(count > 0, count, count > 0 ? "" : "数据库受影响行数为0！");
        }
        /// <summary>
        /// 修改数据。必须传入Id
        /// </summary>
        public BaseResult UptUser(TestTableParam param)
        {
            if (param.Id == null)
            {
                return new BaseResult(false, null, "参数错误！");
            }
            var model = DB.MySql.From<TableMysql>().Where(d => d.Id == param.Id).First();
            if (model == null)
            {
                return new BaseResult(false, null, "不存在要修改的数据！");
            }
            model.Name = param.Name ?? model.Name;
            model.IDNumber = param.IDNumber ?? model.IDNumber;
            model.MobilePhone = param.MobilePhone ?? model.MobilePhone;
            var count = DB.MySql.Update<TableMysql>(model);
            return new BaseResult(true);
        }
    }
}
