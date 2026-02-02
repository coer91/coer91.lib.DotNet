using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace coer91
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            string message = context.Exception.InnerException?.Message ?? context.Exception.Message;
            context.Result = new ObjectResult(message) { StatusCode = 500 };
        }
    }
} 