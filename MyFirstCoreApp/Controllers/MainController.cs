using Microsoft.AspNetCore.Mvc;

namespace MyFirstCoreApp.Controllers
{
    [Route("api")]
    public class MainController : Controller
    {
        //[Route("{*url}", Order = 999)]
        //public IActionResult CatchAll()
        //{
        //    return NotFound(new ASCIIify().err404);
        //}

        //[Route("api")]
        [HttpGet]
        public IActionResult Get()
        {
            string cool = new ASCIIify().gravity;
            return Ok(cool);
        }
    }
}
