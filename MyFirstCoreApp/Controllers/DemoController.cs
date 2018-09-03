using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http.Headers;

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
        /// <summary>
        /// Base route for the DEMO controller
        /// </summary>
        /// <returns>
        /// GRAVITY
        /// </returns>
        [HttpGet]
        public IActionResult Get()
        {
            string cool = new ASCIIify().gravity;
            return Ok(cool);
        }

        /// <summary>
        /// Given a connection string to the specified DB, run any SQL Server commands.
        /// </summary>
        /// <returns>JSON object of the result table</returns>
        [HttpGet("SQLify")]
        public IActionResult SQLifyDemo()
        {
            if (System.IO.File.Exists("cnstring.txt"))
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
            else
            {
                return NotFound(@"This demo only works if you have a connection string provided (or cnstring.txt) and a valid SQL command");
            }

        }

        /// <summary>
        ///  COnvert your classes,datatables, and objects to strings for HTTP responses
        /// </summary>
        /// <returns>
        /// JSON string of a user-defined class Object
        /// </returns>
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

        /// <summary>
        ///  Connect your API to the SMTP server of your choice for emails.
        /// </summary>
        /// <param name="SMTP">SMTP Server used</param>
        /// <param name="SMTPUser">Full email of the SMTP server above (user@server.com)</param>
        /// <param name="SMTPPassword">Not ideal to use password in GET parameter, but this is just a demo right? </param>
        /// <returns>HTTP result codes</returns>
        [HttpGet("Mailify")]
        public IActionResult MailifyDemo(string SMTP, string SMTPUser, string SMTPPassword)
        {
            Mailify mail = new Mailify(SMTP, SMTPUser, SMTPPassword, "FROM@junk.com");
            string err = mail.send("TARGETEMAIL@gmail.com", "SUBJECT", "BODY");
            if (err == "") { return Ok("Message Sent!"); } else { return BadRequest(err); }
        }

        /// <summary>
        /// Encrypt your objects with up-to-date .netCore encryption standards.
        /// </summary>
        /// <returns>
        /// Demo encrypted "Secret Message" with the hash of "YOUR_HASH_HERE" used
        /// </returns>
        [HttpGet("Cryptify")]
        public IActionResult CryptifyDemo()
        {
            Cryptify crypt = new Cryptify("YOUR_HASH_HERE");
            string val = crypt.encode("Secret Message");
            string rtn = crypt.decode(val);
            return Ok(rtn + " ==>>" + val);

        }

        /// <summary>
        /// Execute AJAX commands (the current route uses COUCH db as demo)
        /// </summary>
        /// <returns>
        /// STRING of AJAX results
        /// </returns>
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

        /// <summary>
        /// Using a named connection defined at the [Program.cs] level ensures a singleton approach to http connectionns 
        /// rather than the slower open-and-dispose approach.
        /// </summary>
        /// <returns>
        /// JSON String of the inserted object (this demo route uses couch get command)
        /// </returns>
        [HttpGet("AJAXify/named")]
        public async Task<ActionResult> GetAjaxNamed()
        {
            var client = _httpClientFactory.CreateClient("couch");
            string result = await client.GetStringAsync("/users");
            return Ok(result);
        }

        /// <summary>
        /// AJAX commands also allows for POSt commands
        /// </summary>
        /// <param name="user"></param>
        /// <returns>
        /// JSON String of the inserted object (this demo route uses couch insert command)
        /// </returns>
        [HttpPost("AJAXify/post")]
        public async Task<ActionResult> PostAjaxNamed([FromBody] User user) // <<== When using [FromBody], Content-Type must be application/json.
        {
            AJAXify ajax = new AJAXify();
            ajax.addHeader("Content-Type", "application/json");
            //User body = new MyFirstCoreApp.User();
            //body.Name = "NAME";
            //body.Role = "ROLE";
            var rsp = await ajax.post("http://127.0.0.1:5984/demo", user);
            return Ok(rsp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            if(files.Count == 0)
            {
                return BadRequest("No files found.  Ensure that the name in the key:value pair of your file is called [files]");
            }
            else
            {
                Uploadify uploads = new Uploadify("target path");
                return Ok(uploads.UploadFilesAsync(files));
            }
        }
    }
}