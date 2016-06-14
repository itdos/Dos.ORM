using Dos.Common;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OAA.DataAccess.Entities;

namespace Dos.ORM.Test
{
    class Program
    {
        /// <summary>
        /// 请不要运行此Test项目，此Test仅仅是本人测试用。
        /// 另外有完整的Demo项目：http://git.oschina.net/ITdos/Dos.ORM.Demo
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //请不要运行此Test项目，此Test仅仅是本人测试用。
            //另外有完整的Demo项目：http://git.oschina.net/ITdos/Dos.ORM.Demo

            Console.WriteLine("请不要运行此Test项目，此Test仅仅是本人测试用。");
            Console.WriteLine("另外有完整的Demo项目：http://git.oschina.net/ITdos/Dos.ORM.Demo");
            //return;
            var db = new DbSession(DatabaseType.MySql, "Data Source=192.168.2.150;Database=OAA;User Id=root;Password=root;Convert Zero Datetime=True;Allow Zero Datetime=True;");

            var model = db.From<BizHouse>().Where(d => d.Id == Guid.Parse("d4bdf13a-333c-40e2-b2bc-a6295215f672")).First();
            db.RegisterSqlLogger(SqlOg);
            model.AttachAll();
            model.Id = Guid.NewGuid();
            var count = db.Insert(model);
            return;


            
            var a = CmsNews._.Code != "111" && CmsNews._.Number != "222" && CmsNews._.AllCode != "333";
            var list20160429 = db.From<CmsNews>("A")
                 //.InnerJoin<CmsNews>((a, b) => a.Id == b.Id, "B")
                 .Where(a)
                 .Top(10)
                 .ToList();
            //db.Delete<CmsNews>(d => d.Id.In(1,2,3));
            var uptModeol = new CmsNews();
            uptModeol.Code = "111";
            uptModeol.AllCode = "222";
            db.Update<CmsNews>(uptModeol, CmsNews._.Code == "111" && CmsNews._.Code == "222");

            #region 测试自连接

            //var ll = Db.MySql.From<TestTable>()
            //    .LeftJoin<TestTable>((a, b) => a.Id == b.Id)
            //    .ToList();

            #endregion

            #region 测试

            //var a20160426 = JSON.ToJSON(list);
            //var a201604262 = JsonConvert.SerializeObject(list);
            //list[0].Id = Guid.NewGuid();
            //list[0].AttachAll();
            //var a = list[0].GetModifyFields();
            //var b = list[1].GetModifyFields();
            //list[0].Id = Guid.NewGuid();
            //var aaaa = TestTableRepository.Insert(list[0]);

            #endregion

            #region 测试事务

            //var trans = Db.Context.BeginTransaction();
            //var mmmm = new TestTable1();
            //try
            //{
            //    trans.Delete(new List<TestTable1>());
            //    Db.Context.Delete(trans, new List<TestTable1>());
            //    trans.Update(mmmm);
            //    throw new Exception("xxxxxxxx");
            //    trans.Commit();
            //}
            //catch (Exception)
            //{
            //    trans.Rollback();
            //}
            //finally
            //{
            //    trans.Close();
            //}
            //var aaaaaa = GetAaa();
            //Aaa(new List<TestTable1>());
            //Aaa(trans, new List<TestTable1>());
            //Aaa(aaaaaa);

            #endregion

            #region 测试表名非dbo用户名

            //var list20160408 = Db.dbSql.From<CmsTitleTitlePartRecord>()
            //   // .InnerJoin<CmsTitleTitlePartRecord>((a,b)=>a.Id == b.Id)
            //  // .InnerJoin("", new WhereClip())
            //    .Select(d => new { d.Id, d.Title }).Where(d => d.Id == 8).ToList();
            //var list201604082 = Db.dbSql.Insert<CmsTitleTitlePartRecord>(new CmsTitleTitlePartRecord()
            //{
            //    Id = new Random().Next(1000,10000),
            //    Title = "测试"
            //});
            //var list2016040823 = Db.dbSql.Update<CmsTitleTitlePartRecord>(new CmsTitleTitlePartRecord()
            //{
            //    Id = 8,
            //    Title = "修改为测试"
            //});
            //var list20160414 = Db.dbSql.FromSql("select top 10 * from Cms_Settings_ContentPartFieldDefinitionRecord WHERE 1=2 AND Id=@id")
            //    .AddInParameter("@id",DbType.String, 999999).ToList<CmsTitleTitlePartRecord>();
            //var list21321321 = Db.Context.From<TestTable1>().Select(d => d.MobilePhone).ToList<string>();

            #endregion
        }
        static void SqlOg(string sql)
        {
            LogHelper.Debug(sql, "SQL日志_");
        }
    }
}
