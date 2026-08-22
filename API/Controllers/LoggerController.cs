using API.AutoMappers;
using coer91.NET;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoggerController(AutoMapper _mapper) : ControllerBase
    {
        [HttpGet] 
        [Route("[action]")]
        public async Task<ActionResult> Information() 
        {
            //var x = AppContext.BaseDirectory; 
            Logger.Information("This is an information log.");
            return Ok("");
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult> Mappers()
        {

            var x = new List<User>()
            {
                new() { Id = 1, Name = "John Doe"   },
                new() { Id = 2, Name = "John Doe 2" },
                new() { Id = 3, Name = "John Doe 3" },
            };
             
            var xxx = _mapper.ToDTO(x);

            return Ok();
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
