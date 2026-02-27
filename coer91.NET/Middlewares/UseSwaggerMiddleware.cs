using Microsoft.AspNetCore.Builder; 
using Swashbuckle.AspNetCore.SwaggerUI;

namespace coer91.NET
{
    public static class UseSwaggerMiddleware
    { 
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, bool showInProduction = false)
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