using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business;
using Model.Base;
using UI.Handler;

namespace UI.Controllers
{
    [ErrorLog]
    public class HomeController : Controller
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
            var result = new TestTableLogic().GetUser(param);
            var test = Json(result);
            return test;
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
