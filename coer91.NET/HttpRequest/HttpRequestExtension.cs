using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters; 

namespace coer91.NET
{
    public static class HttpRequestExtension
    {
        public static HttpRequestDTO ToHttpRequest(this ActionExecutingContext context)
            => HttpRequest.ToHttpRequest(context?.HttpContext);

        public static HttpRequestDTO ToHttpRequest(this ActionExecutedContext context)
            => HttpRequest.ToHttpRequest(context?.HttpContext);

        public static HttpRequestDTO ToHttpRequest(this ExceptionContext context)
            => HttpRequest.ToHttpRequest(context?.HttpContext);

        public static HttpRequestDTO ToHttpRequest(this IHttpContextAccessor context)
            => HttpRequest.ToHttpRequest(context?.HttpContext);

        public static HttpRequestDTO ToHttpRequest(this HttpContext context)
            => HttpRequest.ToHttpRequest(context);
    }
}