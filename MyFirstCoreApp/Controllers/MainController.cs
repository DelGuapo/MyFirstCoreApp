using Microsoft.AspNetCore.Mvc;

namespace MyFirstCoreApp.Controllers
{
    [Route("api")]
    public class MainController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            string cool = new ASCIIify().gravity;
            return Ok(cool);
        }
    }
}
