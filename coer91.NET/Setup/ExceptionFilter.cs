using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace coer91.NET
{ 
    public static class ExceptionFilterService
    {
        public static IServiceCollection AddExceptionFilter(this IServiceCollection filter)
        {
            filter.AddTransient<ExceptionFilter>().AddControllers(config => config.Filters.Add<ExceptionFilter>());
            return filter;
        }

        private class ExceptionFilter : IExceptionFilter
        {
            public void OnException(ExceptionContext context)
            {
                string message = context.Exception.InnerException?.Message ?? context.Exception.Message;
                context.Result = new ObjectResult(message) { StatusCode = 500 };
            }
        }  
    }
} 