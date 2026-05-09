using Microsoft.AspNetCore.Builder;

namespace coer91.NET
{
    public static class SetupMiddleware
    {
        public static IApplicationBuilder UseSetup(this WebApplication app)
        {
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            if(Logger.UseLogger) app.UseLogCode500();
            app.MapControllers();
            app.Run();
            return app;
        }
    }
}