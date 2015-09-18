using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NPOI.HSSF.Record.CF;

namespace Common
{
    public class CustomController : Controller
    {
        protected override JsonResult Json(object data, string contentType,
                  Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            if (behavior == JsonRequestBehavior.DenyGet
                && string.Equals(this.Request.HttpMethod, "GET",
                                 StringComparison.OrdinalIgnoreCase))
                return new JsonResult();
            return new JsonNetResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding
            };
        }
    }
    public class JsonNetResult : JsonResult
    {
        public JsonSerializerSettings SerializerSettings { get; set; }
        public Formatting Formatting { get; set; }
        public JsonNetResult()
        {
            SerializerSettings = new JsonSerializerSettings();
        }
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            var response = context.HttpContext.Response;
            response.ContentType =
                !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;
            if (Data != null)
            {
                var writer = new JsonTextWriter(response.Output)
                {
                    Formatting = Formatting,
                    DateFormatString = "yyyy-MM-dd HH:mm:ss"
                };
                var serializer = JsonSerializer.Create(SerializerSettings);
                serializer.Serialize(writer, Data); writer.Flush();
            }
        }
    }
}
