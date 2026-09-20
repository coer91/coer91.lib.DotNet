using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
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

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new EmbeddedFileProvider(typeof(SwaggerMiddleware).Assembly, "coer91.NET.Setup"),
                RequestPath = "/swagger"
            });

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.DocumentTitle = Security.ProjectName;
                options.DocExpansion(DocExpansion.None);
                options.DefaultModelsExpandDepth(-1);

                if(SwaggerConfigurationBuilder.showDefaultGroup)
                    options.SwaggerEndpoint($"/swagger/api/swagger.json", "WEB API");

                foreach (var group in SwaggerConfigurationBuilder.groupList)
                    options.SwaggerEndpoint($"/swagger/{group}/swagger.json", group);

                options.InjectStylesheet("/swagger/SwaggerMiddleware.css");

                if (Security.IsProduction)
                    options.InjectStylesheet("/swagger/SwaggerMiddleware.prod.css");
            });

            return app;
        }
    }
}