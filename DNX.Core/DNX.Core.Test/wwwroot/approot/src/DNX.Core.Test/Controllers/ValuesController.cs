using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace DNX.Core.Test.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var aaa = string.Empty;
            if (string.IsNullOrWhiteSpace(""))
            {

            }
            //DbSession Context = new DbSession(DatabaseType.SqlServer9, 
            //    @"server=.\sql2008r2;database=DosORMSqlServer;uid=sa;pwd=sa;");
            //var list = Context.FromSql("select * from TestTable").ToDataTable();
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
