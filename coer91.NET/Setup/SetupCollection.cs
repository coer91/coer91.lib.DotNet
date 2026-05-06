using Microsoft.Extensions.DependencyInjection; 

namespace coer91.NET
{
    public static class SetupCollection
    {
        public static IServiceCollection AddSetupCollection(this IServiceCollection service)
        {
             service.AddControllers().AddNewtonsoftJson(setup => setup.SerializerSettings.ContractResolver = new DefaultContractResolver {
     NamingStrategy = new DefaultNamingStrategy()
 });

 service.AddAutoMapper(automapper => { }, AppDomain.CurrentDomain.GetAssemblies());
 
 service.AddEndpointsApiExplorer();
 
 service.AddHttpContextAccessor();
 
 service.AddExceptionFilter();
        }
    }
} 
