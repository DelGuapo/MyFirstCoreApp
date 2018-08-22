using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Net.Http;

namespace MyFirstCoreApp.Controllers
{
    [Route("api/[controller]")]
    public class DemoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DemoController(IHttpClientFactory httpClientFactory)
        {
            /* the IHttpClientFactory is injected for any routes needing AJAX controls */
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Get()
        {
            string cool = new ASCIIify().gravity;
            return Ok(cool);
        }

        [HttpGet("SQLify")]
        public IActionResult SQLifyDemo()
        {
            string cnString = System.IO.File.ReadAllText("cnstring.txt");
            SQLify sql = new SQLify(cnString, true);

            List<SqlParameter> sqlParams = new List<SqlParameter> {
                new SqlParameter("item_class", (object)"ItemCLASS"),
                new SqlParameter("item_name", (object)"ItemName"),
                new SqlParameter("item_value", (object)"ItemVal"),
                new SqlParameter("item_descrip", (object)"ItemDescrip")
            };
            string sqlText = "exec insConfig @item_class,@item_name,@item_value,@item_descrip";
            DataTable rsp = sql.SqlDataTable(sqlText, sqlParams);
            return Ok(new Stringify().fromTable(rsp));
        }


        [HttpGet("Stringify")]
        public IActionResult StringifyDemo()
        {
            demoModel Obj = new demoModel();
            Obj.MyString = "TEST";
            Obj.MyDate = DateTime.Now;
            Obj.MyNumber = 1234123;
            Obj.MyNull = null;
            return Ok(new Stringify().fromObject(Obj));
        }

        [HttpGet("Mailify")]
        public IActionResult MailifyDemo()
        {
            Mailify mail = new Mailify("SMTPserver", "USER@D.COM", "PWD", "FROM@junk.com");
            string err = mail.send("RECIPIENT@EMAIL.COM", "SUBJECT", "BODY");
            if (err == "") { return Ok("Message Sent!"); } else { return BadRequest(err); }
        }

        [HttpGet("Cryptify")]
        public IActionResult CryptifyDemo()
        {
            Cryptify crypt = new Cryptify("YOUR_HASH_HERE");
            string val = crypt.encode("Secret Message");
            string rtn = crypt.decode(val);
            return Ok(rtn + " ==>>" + val);

        }


        [HttpGet("AJAXify")]
        public async Task<IActionResult> AJAXifyDemo()
        {
            AJAXify ajax = new AJAXify();
            var rsp = await ajax.get("http://127.0.0.1:5984/users/1");
            return Ok(rsp);
        }

        [HttpGet("AJAXify/basic")]
        public async Task<ActionResult> GetAjax()
        {
            var client = _httpClientFactory.CreateClient();
            string url = "http://127.0.0.1:5984/users/1";
            var result = await client.GetStringAsync(url);
            return Ok(result);
        }

        [HttpGet("AJAXify/named")]
        public async Task<ActionResult> GetAjaxNamed()
        {
            var client = _httpClientFactory.CreateClient("couch");
            string result = await client.GetStringAsync("/users");
            return Ok(result);
        }

        [HttpPost("AJAXify/post")]
        public async Task<ActionResult> PostAjaxNamed()
        {
            AJAXify ajax = new AJAXify();
            ajax.addHeader("Content-Type", "application/json");
            var rsp = await ajax.post("http://127.0.0.1:5984/demo");
            return Ok(rsp);
        }

    }
}
