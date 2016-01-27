using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business;
using Common;
using DataAccess.Entities;
using DataAccess.Entities.Base;

namespace UI.Controllers
{
    [ErrorLog]
    public class HomeController : CustomController
    {
        #region Oracle、MySql、SqlServer、PostgreSql等
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        public JsonResult GetUser(TestTableParam param)
        {
            var bs = new TestTableLogic().GetUser(param);
            #region 以下逻辑可以重写Json()在内部实现
            //var data = bs.Data as List<TestTable1>;
            ////Map目的是过滤前端不需要的字段，如一张表有100个字段，但是前端grid只需要显示10个字段，那么就有90个字段前端其实是不需要的，所以需要在这里过滤掉那90个字段，即在TableMysql_Page1设置前端需要的字段。
            //var result = Common.Common.Map<TestTable1, TestTable_Page1>(data);
            //bs.Data = result;
            ////var test = Json(result);
            #endregion
            return Json(bs);
        }
        /// <summary>
        /// 新增数据
        /// </summary>
        public JsonResult AddUser(TestTableParam param)
        {
            var result = new TestTableLogic().AddUser(param);
            return Json(result);
        }
        /// <summary>
        /// 修改数据
        /// </summary>
        public JsonResult UptUser(TestTableParam param)
        {
            var result = new TestTableLogic().UptUser(param);
            return Json(result);
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        public JsonResult DelUser(TestTableParam param)
        {
            var result = new TestTableLogic().DelUser(param);
            return Json(result);
        }
        #endregion
    }
}
