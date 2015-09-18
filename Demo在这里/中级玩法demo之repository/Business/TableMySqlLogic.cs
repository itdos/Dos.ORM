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
using DataAccess.Entities.Base;
using DataCache;
using QzCRM.Common;

namespace Business
{
    public class TableMySqlLogic
    {
        /// <summary>
        /// 获取数据。
        /// </summary>
        public BaseResult GetUser(TestTableParam param)
        {
            var where = new Where<TestTable>();
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
            if (param._PageIndex != null && param._PageSize != null)
            {
                //取总数，以计算共多少页。自行考虑将总数缓存。
                dateCount = TableMysqlRepository.Count(where);//.SetCacheTimeOut(10)
            }
            #endregion
            var list = TableMysqlRepository.Query(where, d => d.CreateTime, "desc", null, param._PageSize, param._PageIndex);
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
                return new BaseResult(false, null, Msg.ParamError);
            }
            var model = new TestTable
            {
                Id = Guid.NewGuid(),
                Name = param.Name,
                IDNumber = param.IDNumber,
                MobilePhone = param.MobilePhone,
                CreateTime = DateTime.Now
            };
            var count = TableMysqlRepository.Insert(model);
            //设置缓存
            TableMysqlCache.SetUserModel(model);
            return new BaseResult(count > 0, count, count > 0 ? "" : Msg.Line0);
        }
        /// <summary>
        /// 删除数据。必须传入Id
        /// </summary>
        public BaseResult DelUser(TestTableParam param)
        {
            if (param.Id == null)
            {
                return new BaseResult(false, null, Msg.ParamError);
            }
            var count = TableMysqlRepository.Delete(param.Id);
            //更新缓存
            TableMysqlCache.DelUserModel(param.Id.Value);
            return new BaseResult(count > 0, count, count > 0 ? "" : Msg.Line0);
        }
        /// <summary>
        /// 修改数据。必须传入Id
        /// </summary>
        public BaseResult UptUser(TestTableParam param)
        {
            if (param.Id == null)
            {
                return new BaseResult(false, null, Msg.ParamError);
            }
            //取缓存
            var model = TableMysqlCache.GetUserModel(param.Id.Value);
            if (model == null)
            {
                //如果缓存不存在，则从数据库获取
                model = TableMysqlRepository.First(d => d.Id == param.Id);
            }
            if (model == null)
            {
                return new BaseResult(false, null, Msg.NoExist);
            }
            model.Name = param.Name ?? model.Name;
            model.IDNumber = param.IDNumber ?? model.IDNumber;
            model.MobilePhone = param.MobilePhone ?? model.MobilePhone;
            var count = TableMysqlRepository.Update(model);
            //更新缓存
            TableMysqlCache.DelUserModel(param.Id.Value);
            return new BaseResult(true);
        }
    }
}
