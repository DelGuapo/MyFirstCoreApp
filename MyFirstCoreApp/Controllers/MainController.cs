using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
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
        public IActionResult Get()
        {
            string cool = @"                                              
                         +  .        _            .-.       
                                 3==({)_    .    (   \      
                +  .             _  )`\-|      *  )   \     
                        .       /_`' // |\     .-'     `-a:f: : : : Welcome...
              .                B'/`-'M\_| )   /       .     
                      . *       //       (   /      .       
                               B'         `-'               
            ";

            return Ok(cool);
        }

        /// <summary>
        /// This route is for testing new functions 
        /// </summary>
        /// <returns></returns>
        [HttpGet("demo")]
        public IActionResult Demo()
        {
            /* SQLify */
            //string cnString = System.IO.File.ReadAllText("cnstring.txt");
            //SQLify sql = new SQLify(cnString, true);

            //List<SqlParameter> sqlParams = new List<SqlParameter> {
            //    new SqlParameter("item_class", (object)"ItemCLASS"),
            //    new SqlParameter("item_name", (object)"ItemName"),
            //    new SqlParameter("item_value", (object)"ItemVal"),
            //    new SqlParameter("item_descrip", (object)"ItemDescrip")
            //};
            //string sqlText = "exec insConfig @item_class,@item_name,@item_value,@item_descrip";
            //DataTable rsp = sql.SqlDataTable(sqlText, sqlParams);

            /* MAILify */
            //Mailify mail = new Mailify("SMTPserver", "USER@D.COM", "PWD", "FROM@junk.com");
            //mail.send("RECIPIENT@EMAIL.COM", "SUBJECT", "BODY");

            /* Stringify */
            //return Ok(new Stringify().fromTable(rsp));
            //demoModel Obj = new demoModel();
            //Obj.MyString = "TEST";
            //Obj.MyDate = DateTime.Now;
            //Obj.MyNumber = 1234123;
            //Obj.MyNull = null;
            //return Ok(new Stringify().fromObject(Obj));


            /* Cryptify */
            //Cryptify crypt = new Cryptify("HASH");
            //return Ok(crypt.decode("CfDJ8PEMQfBGFHVAqO2i2UXkP1MrVyp9Rnre2X2tICeXHk4NJQ5rUgDqKBiZPLDgQyzZi4yDrRJZ3BG7vxwZZg7UgIAZ4hyG4fwdQGsN719uW6JGt1t9EWXbhjNTh3A_xO87oA"));

            string rtn = @"
             ____________________________________________________
            /                                                    \
           |    _____________________________________________     |
           |   |                                             |    |
           |   |  C:\> dotnet --info                         |    |
           |   |                                             |    |
           |   |                                             |    |
           |   |                                             |    |
           |   |                                             |    |
           |   |                                             |    |
           |   |                                             |    |
           |   |                                             |    |
           |   |                                             |    |
           |   |                                             |    |
           |   |                                             |    |
           |   |                                             |    |
           |   |_____________________________________________|    |
           |                                                      |
            \_____________________________________________________/
                   \_______________________________________/
                _______________________________________________
             _-'    .-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.  --- `-_
          _-'.-.-. .---.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.--.  .-.-.`-_
       _-'.-.-.-. .---.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-`__`. .-.-.-.`-_
    _-'.-.-.-.-. .-----.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-.-----. .-.-.-.-.`-_
 _-'.-.-.-.-.-. .---.-. .-----------------------------. .-.---. .---.-.-.-.`-_
:-----------------------------------------------------------------------------:
`---._.-----------------------------------------------------------------._.---'
";

            return Ok(rtn);
        }
    }
}
