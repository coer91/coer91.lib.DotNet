using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http; 
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace coer91.NET  
{
    public static class LogRequestMiddleware
    {
        public static IServiceCollection AddLogRequest(this IServiceCollection services)
            => services.AddTransient<LogRequest>();


        public static IApplicationBuilder UseLogRequest(this IApplicationBuilder app)
            => Logger.UseLogger ? app.UseMiddleware<LogRequest>() : app;

        private class LogRequest : IMiddleware
        {
            private static readonly string[] sourceArray = ["POST", "PUT", "PATCH"];

            public async Task InvokeAsync(HttpContext context, RequestDelegate _delegate)
            {
                //Response
                using MemoryStream memoryStream = new();
                Stream contextResponse = context.Response.Body;
                context.Response.Body = memoryStream;

                //Request Body
                string fromBody = string.Empty;

                try
                {
                    if (sourceArray.Contains(context.Request.Method))
                    {
                        context.Request.EnableBuffering();
                        byte[] buffer = new byte[context.Request.ContentLength.HasValue ? (int)context.Request.ContentLength.Value : 0];
                        await context.Request.Body.ReadExactlyAsync(buffer);
                        fromBody = Encoding.UTF8.GetString(buffer);
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
                    if (context.Response.StatusCode >= 400 || !context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
                    {
                        //Get user
                        HttpRequestDTO httpRequestDTO = context.ToHttpRequest();

                        string user      = $"User: {httpRequestDTO?.User}\n"; 
                        string service   = $"{httpRequestDTO.HTTP}: {Security.ProjectName} => {httpRequestDTO.Controller} => {httpRequestDTO.Method}\n";
                        string fromRoute = $"FromRoute: {string.Join(" | ", httpRequestDTO.RouteParams.Select((key, value) => $"{key.Key}={value}"))}\n";
                        string fromQuery = $"FromQuery: {string.Join(" | ", httpRequestDTO.QueryParams.Select((key, value) => $"{key.Key}={value}"))}\n"; 
                                               
                        if (fromBody.Contains("PASSWORD", StringComparison.OrdinalIgnoreCase) || fromBody.Contains("TEMPORARY", StringComparison.OrdinalIgnoreCase)) 
                            fromBody = string.Empty;

                        fromBody = $"FromBody: {fromBody}\n";

                        string response = $"Response: " + streamReader.ReadToEnd();
                        string logger = $"{user}{service}{fromRoute}{fromQuery}{fromBody}{response}";

                        if (context.Response.StatusCode >= 500)
                            Logger.Error(logger);

                        else if (context.Response.StatusCode >= 400)
                            Logger.Warning(logger);

                        else
                            Logger.Information(logger);
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