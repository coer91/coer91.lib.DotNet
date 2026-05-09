using coer91.NET;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentationController(IHttpContextAccessor _httpContextAccessor) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [Route("[action]")]
        public async Task<ActionResult> GetContext() => Ok(_httpContextAccessor.ToHttpRequest());         
    }
} 