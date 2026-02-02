using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace coer91
{
    public static class UseSwaggerMiddleware
    {
        /// <summary>
        /// Swagger UI configuration
        /// </summary> 
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, string title, IWebHostEnvironment environment, bool showInProduction = false)
        {              
            if (environment.IsProduction() && !showInProduction)
                return app; 

            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.DocumentTitle = title;
                options.DocExpansion(DocExpansion.None);
                options.DefaultModelsExpandDepth(-1);
            });

            return app;
        }
    }
}