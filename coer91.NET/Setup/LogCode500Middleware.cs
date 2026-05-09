using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace coer91.NET
{
    public static class LogCode500Middleware
    {
        public static IServiceCollection AddLogCode500(this IServiceCollection services)
            => services.AddTransient<LogCode500>();

               
        public static IApplicationBuilder UseLogCode500(this IApplicationBuilder app)
            => app.UseMiddleware<LogCode500>();

        private class LogCode500 : IMiddleware
        { 
            private static readonly string[] sourceArray = ["POST", "PUT", "PATCH"]; 

            public async Task InvokeAsync(HttpContext context, RequestDelegate _delegate)
            {
                //Response
                using MemoryStream memoryStream = new();
                Stream contextResponse = context.Response.Body;
                context.Response.Body = memoryStream;

                //Request Body
                string requestBody = string.Empty; 
                try
                {
                    if (sourceArray.Contains(context.Request.Method))
                    {
                        context.Request.EnableBuffering();
                        byte[] buffer = new byte[context.Request.ContentLength.HasValue ? (int)context.Request.ContentLength.Value : 0];
                        await context.Request.Body.ReadExactlyAsync(buffer);
                        requestBody = Encoding.UTF8.GetString(buffer);
                        context.Request.Body.Position = 0;
                    }
                }
                catch { }    

                //Next
                await _delegate(context);
                memoryStream.Seek(0, SeekOrigin.Begin);
                using StreamReader streamReader = new(memoryStream);

                //Response                 
                try
                { 
                    if (context.Response.StatusCode >= 500)
                    {
                        //Get user
                        HttpRequestDTO httpRequestDTO = context.ToHttpRequest();
                        string user = $"[User: {httpRequestDTO?.User}";
                        user += string.IsNullOrWhiteSpace(httpRequestDTO.Role) ? $" as {httpRequestDTO.Role}]\n" : "]\n";

                        //Get Service
                        string service = $"[{Security.ProjectName} => {httpRequestDTO.Controller} => {httpRequestDTO.Method}]\n";

                        //Get message
                        string message = streamReader.ReadToEnd();

                        //Get body
                        if (requestBody.Contains("PASSWORD", StringComparison.CurrentCultureIgnoreCase) || requestBody.Contains("TEMPORARY", StringComparison.CurrentCultureIgnoreCase))
                            requestBody = string.Empty;

                        if (!string.IsNullOrWhiteSpace(requestBody))
                            requestBody = $"\nFromBody:\n{requestBody}";
                        
                        Logger.Error($"{service}{user}{message}{requestBody}"); 
                    }

                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await memoryStream.CopyToAsync(contextResponse);
                }
                catch { } 

                context.Response.Body = contextResponse;

                //Close Stream
                streamReader.Close();
                streamReader.Dispose();
                contextResponse.Close();
                contextResponse.Dispose();
                memoryStream.Close();
                memoryStream.Dispose();
            }
        }
    }
}