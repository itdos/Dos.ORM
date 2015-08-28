using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dos.Common;
using Dos.ORM;
using System.Data.Common;
using Common;
using DataAccess;
using DataAccess.Entities;

namespace Business
{
    public class TableMySqlLogic
    {
        /// <summary>
        /// 获取数据。
        /// </summary>
        public BaseResult GetUser(TestTableParam param)
        {
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
            
            #region 是否分页
            var dateCount = 0;
            if (param.pageIndex != null && param.pageSize != null)
            {
                //取总数，以计算共多少页。自行考虑将总数缓存。
                dateCount = new TableMysqlRepository().Count(where);//.SetCacheTimeOut(10)
            }
            #endregion
            var list = new TableMysqlRepository().Query(where, d => d.CreateTime, EnumService.OrderBy.Desc, null, param.pageSize, param.pageIndex);
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
            var count = new TableMysqlRepository().Insert(model);
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
            var count = new TableMysqlRepository().Delete(param.Id);
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
            var count = new TableMysqlRepository().Update(model);
            return new BaseResult(true);
        }
    }
}
