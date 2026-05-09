using coer91.NET;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoggerController : ControllerBase
    {
        [HttpGet] 
        [Route("[action]")]
        public async Task<ActionResult> Information() 
        {
            //var x = AppContext.BaseDirectory; 
            Logger.Information("This is an information log.");
            return Ok("");
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> MakeException([FromBody] object data)
        {
            return StatusCode(500, "MakeException");
        }


        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult> MakeExceptionPatch([FromBody] JsonPatchDocument data)
        {
            return StatusCode(500, "MakeException");
        }


        [HttpPut]
        [Route("[action]")]
        public async Task<ActionResult> MakeExceptionFilter([FromBody] JsonPatchDocument data)
        {
            throw new Exception("MakeExceptionFilter");
        }
    }
}
