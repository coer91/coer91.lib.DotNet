using Microsoft.Extensions.DependencyInjection; 

namespace coer91.NET
{
    public static class SetupCollection
    {
        public static IServiceCollection AddSetupCollection(this IServiceCollection service)
        {
            service.AddControllers().AddNewtonsoftJson();
            service.AddEndpointsApiExplorer();
            service.AddHttpContextAccessor();
            service.AddAutoMapper(automapper => { }, AppDomain.CurrentDomain.GetAssemblies());
            service.AddExceptionFilter();
            return service;
        }
    }
} 