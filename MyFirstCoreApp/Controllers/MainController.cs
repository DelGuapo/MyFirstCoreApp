using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System;
using System.IO;
using System.Text;

namespace MyFirstCoreApp.Controllers
{
    [Route("api")]
    public class MainController : Controller
    {
        /// <summary>
        /// Intro
        /// </summary>
        /// <returns>String</returns>
        [HttpGet]
        public string Get()
        {
            return @"                                              
                         +  .        _            .-.       
                                 3==({)_    .    (   \      
                +  .             _  )`\-|      *  )   \     
                        .       /_`' // |\     .-'     `-a:f: : : : Welcome...
              .                B'/`-'M\_| )   /       .     
                      . *       //       (   /      .       
                               B'         `-'               
            ";
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            string cnString = System.IO.File.ReadAllText("cnstring.txt");
            SQLify sql = new SQLify(cnString, true);
            DataTable rsp = sql.SqlDataTable("exec sp_who2");
            return Ok(new Stringify().ToDictionary(rsp));
        }
    }
}
