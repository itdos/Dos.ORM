using Dos.Common;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using OAA.DataAccess.Entities;
using Standard.DataModel;
using WD.Model;

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

            var db20161029 = new DbSession(DatabaseType.SqlServer9, @"Server=.\sql2008r22;uid=sa;pwd=sa;database=OXunDB;");
           
            db20161029.RegisterSqlLogger(SqlOg);
            List<B_OXunGoods> lo = new List<B_OXunGoods>();
            Where<B_OXunGoods> where = new Where<B_OXunGoods>();
            lo = db20161029.From<B_OXunGoods>()
                .OrderByDescending(d => d.Id)
                .Where(where)
                .Page(20, 2)
                .ToList();
            var a1 = JsonConvert.SerializeObject(lo.First());
            var a2 = JSON.ToJSON(lo.First());
            JavaScriptSerializer js = new JavaScriptSerializer();
            var a3 = js.Serialize(lo.First());

            return;


            var db20160927 = new DbSession(DatabaseType.MySql, "Data Source=127.0.0.1;Database=ITdos;User Id=root;Password=root;Convert Zero Datetime=True;Allow Zero Datetime=True;");
            db20160927.RegisterSqlLogger(SqlOg);
            using (var trans = db20160927.BeginTransaction())
            {
                var count2 = 0;
                count2 += trans.Insert(new BizUser()
                {
                    Id = Guid.NewGuid(),
                    Account = new Random().Next(1, 1000).ToString(),
                    Pwd = "123456",
                    State = 1,
                    CreateTime = DateTime.Now
                });

                count2 += trans.FromSql("insert into Biz_User values (UUID(),'" + new Random().Next(1, 1000).ToString() + "','123456','','','',1,'','',1,null,NOW())").ExecuteNonQuery();

                count2 += db20160927.Insert(trans, new BizUser()
                {
                    Id = Guid.NewGuid(),
                    Account = new Random().Next(1, 1000).ToString(),
                    Pwd = "123456",
                    State = 1,
                    CreateTime = DateTime.Now
                });
                trans.Commit();
                Console.WriteLine("成功插入" + count2 + "条数据。");
            }
            return;

            #region 测试字段名不一致
            var list20160927 = db20160927.From<CmsNews>()
                // .Select(d => new { d.Title, d.Id,d.Summary, d.No })//
                .Top(10).ToList();

            var list201609272 = db20160927.From<CmsNews>()
                .Select(d => new { d.Title, d.Id, Sub = d.Summary, d.Summary, d.No })//
                .Top(10).ToList();

            var list201609273 = db20160927.From<CmsNews>()
                .Select(d => new { d.Title, d.Id, Sub = d.Summary, d.Summary, Noo = d.No })//
                .Top(10).ToList();

            var list201609274 = db20160927.From<CmsNews>()
                .Select(d => new { d.Title, d.Id, Sub = d.Summary, d.Summary, Noo = d.No, d.No })//
                .Top(10).ToList();
            return;
            #endregion



            var db2 = new DbSession(DatabaseType.MySql, "Data Source=192.168.2.15;Database=Standard;User Id=root;Password=root;Convert Zero Datetime=True;Allow Zero Datetime=True;");
            db2.RegisterSqlLogger(SqlOg);
            var count20160815 =
                    db2.From<BizStandardList>()
                        .Select(d => d.QATestItemCount.Sum())
                        .Where(d => d.QAClassA825 == "X00/09".Replace("/", "_"))
                        .SetCacheTimeOut(60 * 60 * 24)
                        .ToScalar<int>();
            Console.WriteLine(count20160815);

            var db = new DbSession(DatabaseType.MySql, "Data Source=192.168.2.150;Database=OAA;User Id=root;Password=root;Convert Zero Datetime=True;Allow Zero Datetime=True;");
            db.RegisterSqlLogger(SqlOg);

            var model = db.From<BizHouse>()
                .Select(d => new { d.All, Name2 = d.Name })
                .AddSelect(
                    db.From<BizHouse>()
                        .LeftJoin<BizHouse>((c, d) => c.Id == d.Id)
                        .Select(d => d.Age)
                        .Top(1)
                )
                .Top(2) 
                .ToFirst();
            var bbbbbbbb = JsonConvert.SerializeObject(model);
            var aaaaaaa = JSON.ToJSON(model);
            model.AttachAll();
            model.Id = Guid.NewGuid();
            var count = db.Insert(model);
            return;



            var a = CmsNews._.Code != "111" && CmsNews._.No != "222" && CmsNews._.AllCode != "333";
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
