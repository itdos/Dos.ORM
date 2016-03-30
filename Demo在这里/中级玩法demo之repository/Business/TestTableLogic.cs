using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dos.Common;
using Dos.ORM;
using System.Data.Common;
using Common;
using DataAccess;
using DataAccess.Base;
using DataAccess.Entities;
using DataAccess.Entities.Base;
using DataCache;
using QzCRM.Common;

namespace Business
{
    public class TestTableLogic
    {
        /// <summary>
        /// 获取数据。此数据会持续增长，所以不建议一次性缓存。建议单个Model实体缓存。
        /// </summary>
        public BaseResult GetUser(TestTableParam param)
        {
            var where = new Where<TestTable1>();
            #region 模糊搜索条件
            if (!string.IsNullOrWhiteSpace(param.SearchName))
            {
                where.And(d => !d.Name.Equals(param.SearchName));
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
                dateCount = TestTableRepository.Count(where);//.SetCacheTimeOut(10)
            }
            #endregion
            var list = TestTableRepository.Query(where, d => d.CreateTime, "desc", null, param._PageSize, param._PageIndex);
            //list[0].Id = Guid.NewGuid();
            //list[0].AttachAll();
            //var a = list[0].GetModifyFields();
            //var b = list[1].GetModifyFields();
            //list[0].Id = Guid.NewGuid();
            //var aaaa = TestTableRepository.Insert(list[0]);
            #region 测试事务
            var trans = Db.Context.BeginTransaction();
            var mmmm = new TestTable1();
            try
            {
                trans.Delete(new List<TestTable1>());
                Db.Context.Delete(trans, new List<TestTable1>());
                trans.Update(mmmm);
                throw new Exception("xxxxxxxx"); 
                trans.Commit();
            }
            catch (Exception)
            {
                trans.Rollback();
            }
            finally
            {
                trans.Close();
            }

            #endregion

            //var aaaaaa = GetAaa();
            Aaa(new List<TestTable1>());
            Aaa(trans, new List<TestTable1>());
            //Aaa(aaaaaa);

            return new BaseResult(true, list, "", dateCount);
        }

        public void Aaa(IList<TestTable1> list)
        {

        }
        public void Aaa(DbTransaction tran, IList<TestTable1> list)
        {

        }

        public IList<TestTable1> GetAaa()
        {
            return new List<TestTable1>();
        }


        public BaseResult GetUserModel(TestTableParam param)
        {
            if (param.Id == null)
            {
                return new BaseResult(false, null, Msg.ParamError);
            }
            //取缓存
            var model = TestTableCache.GetUserModel(param.Id.Value);
            if (model == null)
            {
                //如果缓存不存在，则从数据库获取
                model = TestTableRepository.First(d => d.Id == param.Id);
                TestTableCache.SetUserModel(model);
            }
            return new BaseResult(true, model);
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
            var model = new TestTable1
            {
                Id = Guid.NewGuid(),
                Name = param.Name,
                IDNumber = param.IDNumber,
                MobilePhone = param.MobilePhone,
                CreateTime = DateTime.Now,
                T2 = 0,
                T3 = 0,
                T4= true,
                T7 = 0,
                T9 = 0
            };
            var count = TestTableRepository.Insert(model);
            //设置缓存
            TestTableCache.SetUserModel(model);
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
            var count = TestTableRepository.Delete(param.Id);
            //更新缓存
            TestTableCache.DelUserModel(param.Id.Value);
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
            var model = new TestTable1();
            if (param.Name != null)
                model.Name = param.Name;
            if (param.IDNumber != null)
                model.IDNumber = param.IDNumber;
            if (param.MobilePhone != null)
                model.MobilePhone = param.MobilePhone;
            var count = TestTableRepository.Update(model, d => d.Id == param.Id);
            //更新缓存
            TestTableCache.DelUserModel(param.Id.Value);
            return new BaseResult(true);
        }
    }
}
