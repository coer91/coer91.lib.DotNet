using Microsoft.AspNetCore.Builder; 
using Swashbuckle.AspNetCore.SwaggerUI;

namespace coer91.NET
{
    public static class SwaggerMiddleware
    { 
        public static IApplicationBuilder UseSwagger(this WebApplication app, bool showInProduction)
        {              
            if (Security.IsProduction && !showInProduction)
                return app; 

            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.DocumentTitle = Security.ProjectName;
                options.DocExpansion(DocExpansion.None);
                options.DefaultModelsExpandDepth(-1);
            });

            return app;
        }
    }
}