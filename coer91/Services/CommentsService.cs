using Microsoft.Extensions.DependencyInjection; 

namespace coer91
{
    public static class CommentsService
    {
        public static IServiceCollection AddComments(this IServiceCollection context)
        {
            context.AddSwaggerGen(setup => {
                foreach (string xmlPath in Directory.EnumerateFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly))
                    setup.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }); 

            return context;
        }
    }
}